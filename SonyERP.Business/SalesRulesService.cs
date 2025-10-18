// Path: SonyERP.Business/SalesRulesService.cs
using Microsoft.EntityFrameworkCore;
using SonyERP.Data;
using SonyERP.Models;

namespace SonyERP.Business;

public class SalesRulesService : ISalesRulesService
{
    private readonly ApplicationDbContext _db;
    public SalesRulesService(ApplicationDbContext db) => _db = db;

    public async Task<(bool ok, string message, GameAccount? account)> PickAccountAsync(
        string gameName, Platform platform, CopyType copyType, int branchId)
    {
        // 0) Ø§Ø³ØªØ¹Ù„Ø§Ù… Ø£Ø³Ø§Ø³ÙŠ: Ù†ÙØ³ Ø§Ù„Ù„Ø¹Ø¨Ø©/Ø§Ù„Ù…Ù†ØµÙ‘Ø©/Ø§Ù„ÙØ±Ø¹ + Ø§Ù„Ø­Ø³Ø§Ø¨ Ù…Ø§Ø²Ø§Ù„ Ù…ØªØ§Ø­ + Ù„Ø¯ÙŠÙ‡ Ø±ØµÙŠØ¯ Ù„Ù„Ù†ÙˆØ¹ Ø§Ù„Ù…Ø·Ù„ÙˆØ¨
        var q = _db.GameAccounts
            .Where(a =>
                a.IsAvailable &&
                a.GameName == gameName &&
                a.Platform == platform &&
                a.BranchId == branchId);

        // ÙÙ„ØªØ±Ø© Ø­Ø³Ø¨ Ø§Ù„Ø±ØµÙŠØ¯ Ø§Ù„Ù…ØªÙˆÙØ±
        q = copyType switch
        {
            CopyType.Offline   => platform == Platform.PS5
                ? q.Where(a => a.Ps5OfflineLeft   > 0)
                : q.Where(a => a.Ps4OfflineLeft   > 0),

            CopyType.Secondary => platform == Platform.PS5
                ? q.Where(a => a.Ps5SecondaryLeft > 0)
                : q.Where(a => a.Ps4SecondaryLeft > 0),

            CopyType.Primary   => platform == Platform.PS5
                ? q.Where(a => a.Ps5PrimaryLeft   > 0)
                : q.Where(a => a.Ps4PrimaryLeft   > 0),

            _ => q
        };

        // 1) Ù‚Ø§Ø¹Ø¯Ø© "2 Offline Ù‚Ø¨Ù„ Primary" Ø¹Ù„Ù‰ Ù…Ø³ØªÙˆÙ‰ **Ù†ÙØ³ Ø§Ù„Ø¥ÙŠÙ…ÙŠÙ„**
        const int requiredOfflineBeforePrimary = 2;

        if (copyType == CopyType.Primary)
        {
            // Ø£ÙˆÙ„Ø§Ù‹: Ø­Ø§ÙˆÙ„ Ø¥ÙŠØ¬Ø§Ø¯ Ø­Ø³Ø§Ø¨Ø§Øª Ø­Ù‚Ù‚Øª Ø§Ù„Ø´Ø±Ø· (ØªÙ… Ø¨ÙŠØ¹ 2 Offline Ù„Ù‡Ø°Ø§ Ø§Ù„Ø­Ø³Ø§Ø¨)
            var eligible = await q
                .Select(a => new
                {
                    Acc = a,
                    OfflineSold = _db.Sales
                        .Where(s => s.GameAccountId == a.Id && s.CopyType == CopyType.Offline)
                        .Count()
                })
                .Where(x => x.OfflineSold >= requiredOfflineBeforePrimary)
                .OrderBy(x => x.Acc.CreatedAt)
                .Select(x => x.Acc)
                .FirstOrDefaultAsync();

            if (eligible is not null)
                return (true, "OK", eligible);

            // Ø«Ø§Ù†ÙŠØ§Ù‹: fallback Ù…Ø³Ù…ÙˆØ­ (Ù…Ø¹ Ø§Ø³ØªØ«Ù†Ø§Ø¡) â€” Ø§Ø®ØªØ± Ø§Ù„Ø£Ù‚Ø¯Ù… Ø°Ùˆ Ø±ØµÙŠØ¯ Primary>0
            var fallback = await q
                .OrderBy(a => a.CreatedAt)
                .FirstOrDefaultAsync();

            if (fallback is not null)
                return (true, "[EXCEPTION:PRIMARY_BEFORE_2OFFLINE] ØªÙ… Ø§Ø®ØªÙŠØ§Ø± Ø­Ø³Ø§Ø¨ Primary Ù‚Ø¨Ù„ Ø¥ØªÙ…Ø§Ù… Ø´Ø±Ø· 2 Offline Ù„Ù†ÙØ³ Ø§Ù„Ø¥ÙŠÙ…ÙŠÙ„.", fallback);

            return (false, "Ù„Ø§ ÙŠÙˆØ¬Ø¯ Ø­Ø³Ø§Ø¨ Ù…Ù†Ø§Ø³Ø¨ Ø¨Ù‡Ø°Ù‡ Ø§Ù„Ù…ÙˆØ§ØµÙØ§Øª.", null);
        }

        // 2) Ø§Ù„Ø£Ù†ÙˆØ§Ø¹ Ø§Ù„Ø£Ø®Ø±Ù‰ (Offline/Secondary): FIFO Ù…Ø¹ Ø±ØµÙŠØ¯ > 0
        var acc = await q.OrderBy(a => a.CreatedAt).FirstOrDefaultAsync();
        if (acc is null) return (false, "Ù„Ø§ ÙŠÙˆØ¬Ø¯ Ø­Ø³Ø§Ø¨ Ù…Ù†Ø§Ø³Ø¨ Ø¨Ù‡Ø°Ù‡ Ø§Ù„Ù…ÙˆØ§ØµÙØ§Øª.", null);

        return (true, "OK", acc);
    }
}
