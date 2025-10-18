namespace SonyERP.DAL
{
    using System.Threading.Tasks;
    using SonyERP.Models;

    public interface IGameAccountRepository
    {
        Task AddAsync(GameAccount account);
        Task SaveAsync();
    }
}
