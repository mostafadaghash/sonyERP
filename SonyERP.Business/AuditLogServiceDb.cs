using System.Threading.Tasks;
using SonyERP.Business.Services;
using SonyERP.Data;
using SonyERP.Models;

namespace SonyERP.Business
{
    public class AuditLogServiceDb : IAuditService
    {
        private readonly ApplicationDbContext _db;
        public AuditLogServiceDb(ApplicationDbContext db) => _db = db;

        public async Task LogAsync(AuditEvent evt)
        {
            _db.AuditEvents.Add(evt);
            await _db.SaveChangesAsync();
        }
    }
}
