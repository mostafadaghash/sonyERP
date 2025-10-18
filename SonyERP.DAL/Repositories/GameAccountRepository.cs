using System.Threading.Tasks;
using SonyERP.Data;
using SonyERP.Models;

namespace SonyERP.DAL
{
    public class GameAccountRepository : IGameAccountRepository
    {
        private readonly ApplicationDbContext _db;
        public GameAccountRepository(ApplicationDbContext db) => _db = db;

        public Task AddAsync(GameAccount account)
            => _db.GameAccounts.AddAsync(account).AsTask();

        public Task SaveAsync()
            => _db.SaveChangesAsync();
    }
}
