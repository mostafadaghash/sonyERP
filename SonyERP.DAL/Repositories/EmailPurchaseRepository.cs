using System.Threading.Tasks;
using SonyERP.Data;
using SonyERP.Models;

namespace SonyERP.DAL
{
    public class EmailPurchaseRepository : IEmailPurchaseRepository
    {
        private readonly ApplicationDbContext _db;
        public EmailPurchaseRepository(ApplicationDbContext db) => _db = db;

        public Task AddAsync(EmailPurchase purchase) => _db.EmailPurchases.AddAsync(purchase).AsTask();
        public Task SaveAsync() => _db.SaveChangesAsync();
    }
}
