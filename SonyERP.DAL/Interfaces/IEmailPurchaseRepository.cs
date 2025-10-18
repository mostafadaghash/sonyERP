namespace SonyERP.DAL
{
    using System.Threading.Tasks;
    using SonyERP.Models;

    public interface IEmailPurchaseRepository
    {
        Task AddAsync(EmailPurchase purchase);
        Task SaveAsync();
    }
}
