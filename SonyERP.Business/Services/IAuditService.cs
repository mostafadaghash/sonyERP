using System.Threading.Tasks;
using SonyERP.Models;

namespace SonyERP.Business.Services
{
    public interface IAuditService
    {
        Task LogAsync(AuditEvent evt);
    }
}
