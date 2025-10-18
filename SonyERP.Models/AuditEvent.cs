using System;
using System.ComponentModel.DataAnnotations;

namespace SonyERP.Models
{
    public class AuditEvent
    {
        public int Id { get; set; }

        // Timestamp UTC for consistency
        public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;

        // من نفّذ العملية
        public int UserId { get; set; }

        // ممكن يبقى null حسب السيناريو
        public int? AccountId { get; set; }
        public int? SaleId { get; set; }

        // نوع الإجراء: ShowPassword / Copy2FA / RequestAccountOK / SaleCreated / ...
        [Required]
        [MaxLength(64)]
        public string Action { get; set; } = default!;

        // تفاصيل إضافية
        [MaxLength(2000)]
        public string? Details { get; set; }
    }
}
