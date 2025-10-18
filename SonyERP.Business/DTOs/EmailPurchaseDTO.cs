using SonyERP.Models;

namespace SonyERP.Business.DTOs
{
    public class EmailPurchaseDTO
    {
        public int SupplierId { get; set; }
        public int BranchId { get; set; }
        public string GameName { get; set; } = null!;
        public Platform Platform { get; set; }

        public string AccountEmail { get; set; } = null!;
        public string AccountPassword { get; set; } = null!;
        public string? TwoFactorKey { get; set; }

        public decimal Cost { get; set; }

        public int InitialOfflineQuota { get; set; } = 2;
        public int InitialPrimaryQuota { get; set; } = 1;
        public int InitialSecondaryQuota { get; set; } = 0;

        public string? Notes { get; set; }
    }
}
