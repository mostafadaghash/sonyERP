using System;

namespace SonyERP.Models
{
    public class EmailPurchase
    {
        public int Id { get; set; }
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; } = null!;
        public int BranchId { get; set; }

        public string GameName { get; set; } = null!;
        public Platform Platform { get; set; }

        public string AccountEmail { get; set; } = null!;
        public string AccountPassword { get; set; } = null!;
        public string? TwoFactorKey { get; set; }

        public decimal Cost { get; set; }
        public DateTime PurchaseDateUtc { get; set; } = DateTime.UtcNow;

        public int InitialOfflineQuota { get; set; } = 2;
        public int InitialPrimaryQuota { get; set; } = 1;
        public int InitialSecondaryQuota { get; set; } = 0;

        public string? Notes { get; set; }
        public int CreatedByUserId { get; set; }
    }
}
