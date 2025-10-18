namespace SonyERP.Business;

public class AuthorizationService : IAuthorizationService
{
    public Task<bool> CanSellAsync(int employeeUserId, int branchId)
        => Task.FromResult(true); // TODO: ÙØ¹Ù‘Ù„ Ø£Ø¯ÙˆØ§Ø±/ØªØµØ§Ø±ÙŠØ­ Ù…Ù† Ø§Ù„Ø¯Ø§ØªØ§
}
