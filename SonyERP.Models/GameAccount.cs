using System;
using System.ComponentModel.DataAnnotations;

namespace SonyERP.Models
{
    public class GameAccount
    {
        public int Id { get; set; }

        [Required]
        public string GameName { get; set; } = default!;

        [Required]
        public Platform Platform { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; } = default!;

        [Required]
        public string Password { get; set; } = default!;

        // جديد: مفتاح/سر 2FA (اختياري)
        public string? TwoFactorKey { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsAvailable { get; set; } = true;

        // أرصدة PS4
        public int Ps4OfflineLeft   { get; set; }
        public int Ps4SecondaryLeft { get; set; }
        public int Ps4PrimaryLeft   { get; set; }

        // أرصدة PS5
        public int Ps5OfflineLeft   { get; set; }
        public int Ps5SecondaryLeft { get; set; }
        public int Ps5PrimaryLeft   { get; set; }

        // الفرع
        public int BranchId { get; set; }

        // أسعار اختيارية
        public decimal? DefaultSellPrice  { get; set; }
        public decimal? PurchasePrice     { get; set; }
        public decimal? StandardSellPrice { get; set; }
    }
}
