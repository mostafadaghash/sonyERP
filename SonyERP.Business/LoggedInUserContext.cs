// Path: SonyERP.Business/LoggedInUserContext.cs
using SonyERP.Business.Services;

namespace SonyERP.Business
{
    /// <summary>
    /// ÙŠØ­Ù…Ù„ Ù‡ÙˆÙŠØ© Ø§Ù„Ù…Ø³ØªØ®Ø¯Ù… Ø§Ù„Ø­Ø§Ù„ÙŠ Ø£Ø«Ù†Ø§Ø¡ Ø¹Ù…Ø± Ø§Ù„ØªØ·Ø¨ÙŠÙ‚.
    /// ÙŠÙØ³Ø¬Ù‘Ù„ ÙƒÙ€ Singleton ÙˆÙŠÙØ­Ù‚Ù† Ø£ÙŠØ¶Ù‹Ø§ ÙƒÙ€ ICurrentUserContext.
    /// </summary>
    public class LoggedInUserContext : ICurrentUserContext
    {
        public int UserId { get; private set; }
        public int BranchId { get; private set; }
        public bool IsAdmin { get; private set; }

        /// <summary>
        /// Ø§Ø³ØªØ¯Ø¹Ù‡Ø§ Ø¨Ø¹Ø¯ Ù†Ø¬Ø§Ø­ ØªØ³Ø¬ÙŠÙ„ Ø§Ù„Ø¯Ø®ÙˆÙ„ Ù„ØªØ«Ø¨ÙŠØª Ù‡ÙˆÙŠØ© Ø§Ù„Ù…Ø³ØªØ®Ø¯Ù….
        /// </summary>
        public void Set(int userId, int branchId, bool isAdmin)
        {
            UserId = userId;
            BranchId = branchId;
            IsAdmin = isAdmin;
        }
    }
}
