// Path: SonyERP.Business/SaleManager.cs
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SonyERP.Business.Services;          // IAuditService
using SonyERP.Data;
using SonyERP.Models;

namespace SonyERP.Business
{
    public class SaleManager : ISaleManager
    {
        private readonly ISalesRulesService _rules;
        private readonly IAuthorizationService _auth;
        private readonly ApplicationDbContext _db;
        private readonly IUnitOfWork _uow;
        private readonly IAuditService _audit;

        public SaleManager(
            ISalesRulesService rules,
            IAuthorizationService auth,
            ApplicationDbContext db,
            IUnitOfWork uow,
            IAuditService audit)
        {
            _rules = rules;
            _auth = auth;
            _db = db;
            _uow = uow;
            _audit = audit;
        }

        public async Task<SaleResultDTO> SellAsync(SaleRequestDTO req)
        {
            // صلاحيات
            if (!await _auth.CanSellAsync(req.EmployeeUserId, req.BranchId))
                return new(false, "الموظف غير مُخوّل لإتمام عملية البيع.");

            // اختيار الحساب حسب القواعد
            var (ok, msg, acc) = await _rules.PickAccountAsync(req.GameName, req.Platform, req.CopyType, req.BranchId);
            if (!ok || acc is null) return new(false, msg);

            // هل هذه حالة استثناء: Primary قبل 2 Offline؟
            bool isExceptionPrimaryBefore2 =
                !string.IsNullOrEmpty(msg) &&
                msg.Contains("[EXCEPTION:PRIMARY_BEFORE_2OFFLINE]", StringComparison.Ordinal);

            // 1) خصم الرصيد للنسخة المطلوبة
            if (!DecrementQuota(acc, req.Platform, req.CopyType, out string quotaError))
                return new(false, quotaError);

            // 2) لو جميع الأرصدة للمنصة أصبحت صفر — أغلق الحساب (غير متاح)
            if (IsPlatformDepleted(acc, req.Platform))
                acc.IsAvailable = false;

            // 3) إنشاء سجل البيع
            var sale = new Sale
            {
                GameAccountId = acc.Id,
                CopyType = req.CopyType,
                Price = req.Price,
                EmployeeUserId = req.EmployeeUserId,
                CustomerId = req.CustomerId,
                BranchId = req.BranchId,
                SoldAt = DateTime.UtcNow
            };

            await _db.Sales.AddAsync(sale);
            _db.GameAccounts.Update(acc);

            // احفظ أولاً للحصول على sale.Id الصحيح
            await _uow.SaveChangesAsync();

            // 4) Audit — سجل عملية البيع (بدون BranchId/GameAccountId لأنهم غير موجودين بموديلك الحالي)
            await _audit.LogAsync(new AuditEvent
            {
                Action = "SELL",
                Details = $"Game={req.GameName}, Platform={req.Platform}, CopyType={req.CopyType}, Price={req.Price}",
                UserId = req.EmployeeUserId,
                TimestampUtc = DateTime.UtcNow,
                SaleId = sale.Id
            });

            // 5) Audit للاستثناء إن وجد
            if (isExceptionPrimaryBefore2 && req.CopyType == CopyType.Primary)
            {
                await _audit.LogAsync(new AuditEvent
                {
                    Action = "EXCEPTION_PRIMARY_BEFORE_2OFFLINE",
                    Details = $"Primary sold before 2 Offline. Game={req.GameName}, Platform={req.Platform}, AccountId={acc.Id}",
                    UserId = req.EmployeeUserId,
                    TimestampUtc = DateTime.UtcNow,
                    SaleId = sale.Id
                });
            }

            return new(true,
                isExceptionPrimaryBefore2
                    ? "تمت عملية البيع بنجاح (تنبيه: Primary خرجت قبل 2 Offline وتم تسجيل ذلك)."
                    : "تمت عملية البيع بنجاح.",
                sale.Id,
                acc.Id);
        }

        private static bool DecrementQuota(GameAccount acc, Platform platform, CopyType type, out string error)
        {
            error = "";

            if (platform == Platform.PS5)
            {
                switch (type)
                {
                    case CopyType.Offline:
                        if (acc.Ps5OfflineLeft <= 0) { error = "لا يوجد رصيد Offline متاح لهذا الحساب (PS5)."; return false; }
                        acc.Ps5OfflineLeft--; return true;

                    case CopyType.Secondary:
                        if (acc.Ps5SecondaryLeft <= 0) { error = "لا يوجد رصيد Secondary متاح لهذا الحساب (PS5)."; return false; }
                        acc.Ps5SecondaryLeft--; return true;

                    case CopyType.Primary:
                        if (acc.Ps5PrimaryLeft <= 0) { error = "لا يوجد رصيد Primary متاح لهذا الحساب (PS5)."; return false; }
                        acc.Ps5PrimaryLeft--; return true;
                }
            }
            else // PS4
            {
                switch (type)
                {
                    case CopyType.Offline:
                        if (acc.Ps4OfflineLeft <= 0) { error = "لا يوجد رصيد Offline متاح لهذا الحساب (PS4)."; return false; }
                        acc.Ps4OfflineLeft--; return true;

                    case CopyType.Secondary:
                        if (acc.Ps4SecondaryLeft <= 0) { error = "لا يوجد رصيد Secondary متاح لهذا الحساب (PS4)."; return false; }
                        acc.Ps4SecondaryLeft--; return true;

                    case CopyType.Primary:
                        if (acc.Ps4PrimaryLeft <= 0) { error = "لا يوجد رصيد Primary متاح لهذا الحساب (PS4)."; return false; }
                        acc.Ps4PrimaryLeft--; return true;
                }
            }

            error = "نوع النسخة غير معروف.";
            return false;
        }

        private static bool IsPlatformDepleted(GameAccount acc, Platform platform)
        {
            return platform == Platform.PS5
                ? (acc.Ps5OfflineLeft <= 0 && acc.Ps5SecondaryLeft <= 0 && acc.Ps5PrimaryLeft <= 0)
                : (acc.Ps4OfflineLeft <= 0 && acc.Ps4SecondaryLeft <= 0 && acc.Ps4PrimaryLeft <= 0);
        }
    }
}
