using System;
using System.Threading.Tasks;
using SonyERP.Models;

namespace SonyERP.Business.Services
{
    public static class IAuditServiceExtensions
    {
        // توقيع قديم: (action, userId, accountId, saleId, details)
        public static Task LogAsync(this IAuditService audit, string action, int userId, int? accountId, int? saleId, string details)
        {
            if (audit == null) throw new ArgumentNullException(nameof(audit));
            var evt = new AuditEvent
            {
                Action = action,
                Details = details,
                UserId = userId,
                TimestampUtc = DateTime.UtcNow
            };
            // لو AuditEvent عندك فيه SaleId أو غيره، ضيفه هنا
            // evt.SaleId = saleId ?? 0;
            return audit.LogAsync(evt);
        }
    }
}
