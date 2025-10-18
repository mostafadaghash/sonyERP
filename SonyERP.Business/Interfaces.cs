using SonyERP.Models;

namespace SonyERP.Business;

public interface ICurrentUserContext
{
    int UserId { get; }
    int BranchId { get; }
    bool IsAdmin { get; }
}

public interface IAuthorizationService
{
    Task<bool> CanSellAsync(int employeeUserId, int branchId);
}

public interface ISalesRulesService
{
    Task<(bool ok, string message, GameAccount? account)> PickAccountAsync(
        string gameName, Platform platform, CopyType copyType, int branchId);
}

public interface ISaleManager
{
    Task<SaleResultDTO> SellAsync(SaleRequestDTO request);
}
