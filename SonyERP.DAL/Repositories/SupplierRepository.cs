using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SonyERP.Data;
using SonyERP.Models;

namespace SonyERP.DAL
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly ApplicationDbContext _db;
        public SupplierRepository(ApplicationDbContext db) => _db = db;

        public Task<Supplier?> GetByIdAsync(int id) =>
            _db.Suppliers.FirstOrDefaultAsync(x => x.Id == id && x.IsActive);

        public Task<List<Supplier>> GetActiveAsync() =>
            _db.Suppliers.Where(x => x.IsActive).OrderBy(x => x.Name).ToListAsync();

        public Task AddAsync(Supplier supplier) => _db.Suppliers.AddAsync(supplier).AsTask();
        public Task SaveAsync() => _db.SaveChangesAsync();
    }
}
