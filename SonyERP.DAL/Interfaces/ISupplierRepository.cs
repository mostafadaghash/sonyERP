namespace SonyERP.DAL
{
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using SonyERP.Models;

    public interface ISupplierRepository
    {
        Task<Supplier?> GetByIdAsync(int id);
        Task<List<Supplier>> GetActiveAsync();
        Task AddAsync(Supplier supplier);
        Task SaveAsync();
    }
}
