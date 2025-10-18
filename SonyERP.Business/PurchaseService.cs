using System;
using System.Threading.Tasks;
using SonyERP.Models;                 // GameAccount, AuditEvent, Platform
using SonyERP.DAL;                    // IEmailPurchaseRepository, ISupplierRepository, IGameAccountRepository
using SonyERP.Business.DTOs;          // EmailPurchaseDTO
using SonyERP.Business.Services;      // IAuditService

namespace SonyERP.Business
{
    public class PurchaseService
    {
        private readonly IEmailPurchaseRepository _purchaseRepo;
        private readonly ISupplierRepository _supplierRepo;
        private readonly IGameAccountRepository _gameAccountRepo;
        private readonly IAuditService _audit;

        public PurchaseService(
            IEmailPurchaseRepository purchaseRepo,
            ISupplierRepository supplierRepo,
            IGameAccountRepository gameAccountRepo,
            IAuditService audit)
        {
            _purchaseRepo = purchaseRepo;
            _supplierRepo = supplierRepo;
            _gameAccountRepo = gameAccountRepo;
            _audit = audit;
        }

        public async Task<int> PurchaseEmailAccountAsync(EmailPurchaseDTO dto, int currentUserId)
        {
            var supplier = await _supplierRepo.GetByIdAsync(dto.SupplierId)
                           ?? throw new InvalidOperationException("المورد غير موجود أو غير مفعل.");

            // 1) حفظ عملية الشراء (EmailPurchase)
            var purchase = new EmailPurchase
            {
                SupplierId = dto.SupplierId,
                BranchId = dto.BranchId,
                GameName = dto.GameName,
                Platform = dto.Platform,
                AccountEmail = dto.AccountEmail,
                AccountPassword = dto.AccountPassword,
                TwoFactorKey = dto.TwoFactorKey,
                Cost = dto.Cost,
                Notes = dto.Notes,
                InitialOfflineQuota = dto.InitialOfflineQuota,
                InitialPrimaryQuota = dto.InitialPrimaryQuota,
                InitialSecondaryQuota = dto.InitialSecondaryQuota,
                CreatedByUserId = currentUserId,
                PurchaseDateUtc = DateTime.UtcNow
            };

            await _purchaseRepo.AddAsync(purchase);

            // 2) إنشاء GameAccount جديد وفق منصة الحساب
            var newAccount = new GameAccount
            {
                BranchId = dto.BranchId,
                GameName = dto.GameName,
                Platform = dto.Platform,
                Email = dto.AccountEmail,
                Password = dto.AccountPassword,
                TwoFactorKey = dto.TwoFactorKey,
                IsAvailable = true
            };

            // وضع الكوتا الابتدائية حسب المنصة
            if (dto.Platform == Platform.PS5)
            {
                newAccount.Ps5OfflineLeft   = dto.InitialOfflineQuota;
                newAccount.Ps5PrimaryLeft   = dto.InitialPrimaryQuota;
                newAccount.Ps5SecondaryLeft = dto.InitialSecondaryQuota;

                // تأكد أن قيم PS4 = 0 (لو الموديل يعرّفها افتراضيًا بصفَر، تمام)
                newAccount.Ps4OfflineLeft   = 0;
                newAccount.Ps4PrimaryLeft   = 0;
                newAccount.Ps4SecondaryLeft = 0;
            }
            else // PS4
            {
                newAccount.Ps4OfflineLeft   = dto.InitialOfflineQuota;
                newAccount.Ps4PrimaryLeft   = dto.InitialPrimaryQuota;
                newAccount.Ps4SecondaryLeft = dto.InitialSecondaryQuota;

                newAccount.Ps5OfflineLeft   = 0;
                newAccount.Ps5PrimaryLeft   = 0;
                newAccount.Ps5SecondaryLeft = 0;
            }

            await _gameAccountRepo.AddAsync(newAccount);

            // 3) حفظ (نفس DbContext، يكفي SaveAsync واحد إذا موحّد)
            await _purchaseRepo.SaveAsync();

            // 4) Audit (بدون حقول BranchId/GameAccountId لأنهم مش موجودين في موديلك)
            await _audit.LogAsync(new AuditEvent
            {
                Action = "PURCHASE",
                Details = $"Purchase #{purchase.Id} -> GameAccount '{newAccount.GameName}' ({newAccount.Platform}) by Supplier #{supplier.Id}",
                UserId = currentUserId,
                TimestampUtc = DateTime.UtcNow
            });

            return newAccount.Id;
        }
    }
}
