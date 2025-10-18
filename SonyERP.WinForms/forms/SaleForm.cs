using SonyERP.Business.Services;
// Path: SonyERP.WinForms/forms/SaleForm.cs
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using SonyERP.Business;
using SonyERP.Data;
using SonyERP.Models;

namespace SonyERP.WinForms.Forms
{
    public class SaleForm : Form
    {
        private readonly ISaleManager _saleManager;
        private readonly ISalesRulesService _rules;
        private readonly ApplicationDbContext _db;
        private readonly ICurrentUserContext _user;
        private readonly IAuditService _audit;

        // ÙŠØ³Ø§Ø± (Ø§Ø®ØªÙŠØ§Ø±Ø§Øª Ø§Ù„Ø·Ù„Ø¨)
        private readonly ComboBox _cboGame = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 220 };
        private readonly ComboBox _cboPlatform = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 120 };
        private readonly ComboBox _cboCopyType = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 150 };
        private readonly NumericUpDown _numPrice = new()
        {
            DecimalPlaces = 2,
            Maximum = 1_000_000,
            Minimum = 0,
            Increment = 10,
            Width = 150
        };

        // ÙŠÙ…ÙŠÙ† (Ø¨ÙŠØ§Ù†Ø§Øª Ø§Ù„Ø¹Ù…ÙŠÙ„)
        private readonly TextBox _txtCustName  = new() { PlaceholderText = "Ø§Ø³Ù… Ø§Ù„Ø¹Ù…ÙŠÙ„",  Width = 260 };
        private readonly TextBox _txtCustPhone = new() { PlaceholderText = "Ø±Ù‚Ù… Ø§Ù„ØªÙ„ÙŠÙÙˆÙ†", Width = 200 };
        private readonly TextBox _txtCustAddr  = new() { PlaceholderText = "Ø§Ù„Ø¹Ù†ÙˆØ§Ù†",      Width = 300 };

        // Ù„ÙˆØ­Ø© Ø§Ù„Ù…Ø¹Ø§ÙŠÙ†Ø© â€” ÙØ§Ø±ØºØ© Ø§ÙØªØ±Ø§Ø¶ÙŠÙ‹Ø§
        private readonly TextBox _txtPreviewEmail = new() { ReadOnly = true, Width = 420 };
        private readonly TextBox _txtPreviewPass  = new() { ReadOnly = true, Width = 420 };
        private int? _previewAccountId;

        // Ù„ÙˆØ¬ Ø¨Ø³ÙŠØ· (Ø§Ø®ØªÙŠØ§Ø±ÙŠ)
        private readonly TextBox _log = new() { Multiline = true, Dock = DockStyle.Fill, ScrollBars = ScrollBars.Vertical };

        // Ø£Ø²Ø±Ø§Ø±
        private readonly Button _btnRequest  = new() { Text = "Request",  AutoSize = true };
        private readonly Button _btnSell     = new() { Text = "Sell",      AutoSize = true };
        private readonly Button _btnSuggest  = new() { Text = "Suggest",   AutoSize = true };
        private readonly Button _btnRefresh  = new() { Text = "Refresh",   AutoSize = true };
        private readonly Button _btnClear    = new() { Text = "Clear",     AutoSize = true };

        public SaleForm(
            ISaleManager saleManager,
            ISalesRulesService rules,
            ApplicationDbContext db,
            ICurrentUserContext user,
            IAuditService audit)
        {
            _saleManager = saleManager;
            _rules       = rules;
            _db          = db;
            _user        = user;
            _audit       = audit;

            Text = "Ø¨ÙŠØ¹ Ù†Ø³Ø®Ø© (Sale)";
            WindowState = FormWindowState.Maximized;

            BuildUi();
            WireEvents();

            _ = InitUiAsync();
        }

        private void BuildUi()
        {
            // ØªØ®Ø·ÙŠØ· Ø¹Ø§Ù…: ØµÙ Ø¹Ù„ÙˆÙŠ ÙÙŠÙ‡ (ÙŠÙ…ÙŠÙ† Ø¨ÙŠØ§Ù†Ø§Øª Ø¹Ù…ÙŠÙ„) + (ÙŠØ³Ø§Ø± Ø§Ø®ØªÙŠØ§Ø±Ø§Øª Ø§Ù„Ø¨ÙŠØ¹)
            var top = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                RowCount = 1,
                ColumnCount = 2,
                AutoSize = true,
                Padding = new Padding(10)
            };
            top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            // ÙŠØ³Ø§Ø±: Game/Platform/CopyType/Price + Ø£Ø²Ø±Ø§Ø±
            var left = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = true };
            left.Controls.Add(new Label { Text = "Game", AutoSize = true, Padding = new Padding(0, 10, 4, 0) });
            left.Controls.Add(_cboGame);

            left.Controls.Add(new Label { Text = "Platform", AutoSize = true, Padding = new Padding(12, 10, 4, 0) });
            left.Controls.Add(_cboPlatform);

            left.Controls.Add(new Label { Text = "Copy Type", AutoSize = true, Padding = new Padding(12, 10, 4, 0) });
            left.Controls.Add(_cboCopyType);

            left.Controls.Add(new Label { Text = "Price", AutoSize = true, Padding = new Padding(12, 10, 4, 0) });
            left.Controls.Add(_numPrice);

            left.Controls.Add(_btnSuggest);
            left.Controls.Add(_btnRequest);
            left.Controls.Add(_btnSell);
            left.Controls.Add(_btnRefresh);
            left.Controls.Add(_btnClear);

            // ÙŠÙ…ÙŠÙ†: Ø¨ÙŠØ§Ù†Ø§Øª Ø§Ù„Ø¹Ù…ÙŠÙ„
            var rightOuter = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, FlowDirection = FlowDirection.RightToLeft, WrapContents = false };
            var right = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.TopDown, Padding = new Padding(6) };
            right.Controls.Add(new Label { Text = "Ø§Ø³Ù… Ø§Ù„Ø¹Ù…ÙŠÙ„", AutoSize = true });
            right.Controls.Add(_txtCustName);
            right.Controls.Add(new Label { Text = "Ø±Ù‚Ù… Ø§Ù„ØªÙ„ÙŠÙÙˆÙ†", AutoSize = true });
            right.Controls.Add(_txtCustPhone);
            right.Controls.Add(new Label { Text = "Ø§Ù„Ø¹Ù†ÙˆØ§Ù†", AutoSize = true });
            right.Controls.Add(_txtCustAddr);
            rightOuter.Controls.Add(right);

            top.Controls.Add(left, 0, 0);
            top.Controls.Add(rightOuter, 1, 0);

            // ØµÙ†Ø¯ÙˆÙ‚ Ø§Ù„Ù…Ø¹Ø§ÙŠÙ†Ø©
            var grpAccount = new GroupBox
            {
                Text = "Account Preview (ÙŠØ¸Ù‡Ø± Ø¨Ø¹Ø¯ Ø§Ù„Ø¶ØºØ· Ø¹Ù„Ù‰ Request)",
                Dock = DockStyle.Top,
                Height = 140,
                Padding = new Padding(10)
            };
            var grid = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, AutoSize = true };
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            grid.Controls.Add(new Label { Text = "Email:", AutoSize = true }, 0, 0);
            grid.Controls.Add(_txtPreviewEmail, 1, 0);
            grid.Controls.Add(new Label { Text = "Password:", AutoSize = true }, 0, 1);
            grid.Controls.Add(_txtPreviewPass,   1, 1);
            grpAccount.Controls.Add(grid);

            Controls.Add(_log);
            Controls.Add(grpAccount);
            Controls.Add(top);
        }

        private void WireEvents()
        {
            _btnSuggest.Click += async (_, __) => await SuggestPriceAsync();
            _btnRequest.Click += async (_, __) => await UpdateAccountPreviewAsync();
            _btnSell.Click    += async (_, __) => await DoSellAsync();
            _btnRefresh.Click += async (_, __) => { await LoadGamesAsync(); ClearPreview(); await SuggestPriceAsync(); };
            _btnClear.Click   += (_, __) => { ClearPreview(); _numPrice.Value = 0; };

            _cboGame.SelectedIndexChanged     += async (_, __) => { ClearPreview(); await SuggestPriceAsync(); };
            _cboPlatform.SelectedIndexChanged += async (_, __) => { ClearPreview(); await SuggestPriceAsync(); };
            _cboCopyType.SelectedIndexChanged += async (_, __) => { ClearPreview(); await SuggestPriceAsync(); };
        }

        private async Task InitUiAsync()
        {
            // Ù…Ù†ØµÙ‘Ø©
            _cboPlatform.Items.Clear();
            _cboPlatform.Items.Add("PS4");
            _cboPlatform.Items.Add("PS5");
            _cboPlatform.SelectedIndex = 1; // PS5 Ø§ÙØªØ±Ø§Ø¶ÙŠÙ‹Ø§

            // Ù†ÙˆØ¹ Ø§Ù„Ù†Ø³Ø®Ø©
            _cboCopyType.Items.Clear();
            _cboCopyType.Items.Add("Offline");
            _cboCopyType.Items.Add("Primary");
            _cboCopyType.Items.Add("Secondary");
            _cboCopyType.SelectedIndex = 0;

            ClearPreview();
            await LoadGamesAsync();
            await SuggestPriceAsync();
        }

        private async Task LoadGamesAsync()
        {
            try
            {
                var games = await _db.GameAccounts
                    .AsNoTracking()
                    .Where(a => a.BranchId == _user.BranchId)
                    .Select(a => a.GameName)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToListAsync();

                var prev = _cboGame.SelectedItem?.ToString();
                _cboGame.Items.Clear();
                foreach (var g in games) _cboGame.Items.Add(g);

                if (prev != null && games.Contains(prev))
                    _cboGame.SelectedItem = prev;
                else if (_cboGame.Items.Count > 0)
                    _cboGame.SelectedIndex = 0;
                else
                    _cboGame.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show("LoadGames error:\n" + ex, "Sale", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Platform? GetSelectedPlatform() =>
            _cboPlatform.SelectedItem?.ToString() switch
            {
                "PS4" => Platform.PS4,
                "PS5" => Platform.PS5,
                _ => null
            };

        private CopyType? GetSelectedCopyType() =>
            _cboCopyType.SelectedItem?.ToString() switch
            {
                "Offline"   => CopyType.Offline,
                "Primary"   => CopyType.Primary,
                "Secondary" => CopyType.Secondary,
                _ => null
            };

        private string? GetSelectedGame() => _cboGame.SelectedItem?.ToString();

        private void ClearPreview()
        {
            _txtPreviewEmail.Text = string.Empty;
            _txtPreviewPass.Text  = string.Empty;
            _previewAccountId     = null;
        }

        private async Task SuggestPriceAsync()
        {
            try
            {
                var game = GetSelectedGame();
                var platform = GetSelectedPlatform();
                var type = GetSelectedCopyType();
                if (string.IsNullOrWhiteSpace(game) || platform is null || type is null) return;

                // Ø£ÙˆÙ„ Ø­Ø³Ø§Ø¨ Ù…ØªØ§Ø­ FIFO Ù„Ù†ÙØ³ Ø§Ù„Ø§Ø®ØªÙŠØ§Ø±
                var acc = await _db.GameAccounts
                    .AsNoTracking()
                    .Where(a => a.GameName == game && a.Platform == platform && a.BranchId == _user.BranchId && a.IsAvailable)
                    .OrderBy(a => a.CreatedAt)
                    .FirstOrDefaultAsync();

                decimal suggested =
                    (acc?.DefaultSellPrice) ??
                    (type == CopyType.Primary ? 700 :
                     type == CopyType.Secondary ? 600 : 500);

                if (suggested > 0) _numPrice.Value = suggested;
            }
            catch
            {
                // ØªØ¬Ø§Ù‡Ù„ Ø£ÙŠ Ø®Ø·Ø£ ÙÙŠ Ø§Ù„Ø§Ù‚ØªØ±Ø§Ø­ â€” Ù„ÙŠØ³ Ø­Ø±Ø¬Ù‹Ø§
            }
        }

        // Ø²Ø± Request â€” ÙŠØ®ØªØ§Ø± Ø­Ø³Ø§Ø¨ ÙˆÙŠØ¹Ø±Ø¶ Email/Password ÙÙŠ Modal Ø«Ù… ÙŠÙ…Ù„Ø£ Ø§Ù„Ù…Ø¹Ø§ÙŠÙ†Ø© Ø¹Ù†Ø¯ OK
        private async Task UpdateAccountPreviewAsync()
        {
            try
            {
                ClearPreview();

                var game = GetSelectedGame();
                var platform = GetSelectedPlatform();
                var type = GetSelectedCopyType();

                if (string.IsNullOrWhiteSpace(game) || platform is null || type is null)
                {
                    MessageBox.Show("Ø§Ø®ØªØ± Game/Platform/CopyType Ø£ÙˆÙ„Ù‹Ø§.", "Request", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var pick = await _rules.PickAccountAsync(game, platform.Value, type.Value, _user.BranchId);
                if (!pick.ok || pick.account is null)
                {
                    MessageBox.Show(pick.message ?? "Ù„Ø§ ÙŠÙˆØ¬Ø¯ Ø­Ø³Ø§Ø¨ Ù…Ù†Ø§Ø³Ø¨ Ø¨Ù‡Ø°Ù‡ Ø§Ù„Ù…ÙˆØ§ØµÙØ§Øª.", "Request", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // TwoFA (Ù„Ùˆ ÙƒÙ†Øª Ø¶ÙØªÙ‡ ÙÙŠ Ø§Ù„Ù€ Schema) â€” Ù…Ù…ÙƒÙ† ØªÙ‚Ø±Ø£Ù‡ Ù…Ù† pick.account.TwoFactorKey
                string? twofa = pick.account.TwoFactorKey;

                using var dlg = new CredentialDialog(_audit, _user.UserId, pick.account.Id, pick.account.Email, pick.account.Password, twofa);
                var res = dlg.ShowDialog(this);

                if (res == DialogResult.OK)
                {
                    _txtPreviewEmail.Text = pick.account.Email;
                    _txtPreviewPass.Text  = pick.account.Password;
                    _previewAccountId     = pick.account.Id;

                    await _audit.LogAsync("RequestAccountOK", _user.UserId, pick.account.Id, null,
                        $"Game={game}, Platform={platform}, Type={type}");
                }
                else
                {
                    await _audit.LogAsync("RequestAccountCancel", _user.UserId, pick.account.Id, null,
                        "User cancelled credential dialog");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Request error:\n" + ex, "Request", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task<int> EnsureCustomerAsync(string? name, string? phone, string? address)
        {
            if (string.IsNullOrWhiteSpace(name))
                name = "Ø¹Ù…ÙŠÙ„";

            Customer? existing = null;
            if (!string.IsNullOrWhiteSpace(phone))
                existing = await _db.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.Phone == phone);
            if (existing is null)
                existing = await _db.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.Name == name);

            if (existing is not null) return existing.Id;

            var c = new Customer
            {
                Name = name,
                Phone = string.IsNullOrWhiteSpace(phone) ? null : phone,
                Address = string.IsNullOrWhiteSpace(address) ? null : address
            };
            _db.Customers.Add(c);
            await _db.SaveChangesAsync();
            return c.Id;
        }

        private async Task DoSellAsync()
        {
            try
            {
                var game = GetSelectedGame();
                var platform = GetSelectedPlatform();
                var type = GetSelectedCopyType();

                if (string.IsNullOrWhiteSpace(game) || platform is null || type is null)
                {
                    MessageBox.Show("Ù…Ù† ÙØ¶Ù„Ùƒ Ø§Ø®ØªØ± Ø§Ù„Ù„Ø¹Ø¨Ø© ÙˆØ§Ù„Ù…Ù†ØµÙ‘Ø© ÙˆÙ†ÙˆØ¹ Ø§Ù„Ù†Ø³Ø®Ø©.", "Sell", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (_numPrice.Value <= 0)
                {
                    MessageBox.Show("Ù…Ù† ÙØ¶Ù„Ùƒ Ø£Ø¯Ø®Ù„ Ø³Ø¹Ø±Ù‹Ø§ ØµØ§Ù„Ø­Ù‹Ø§.", "Sell", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Ù„Ùˆ Ù…ÙˆØ¸Ù: Ù„Ø§Ø²Ù… ÙŠØ¹Ù…Ù„ Request ÙˆÙŠØ¨ÙŠØ¹ Ù†ÙØ³ Ø§Ù„Ø­Ø³Ø§Ø¨ Ø§Ù„Ù…Ø¹Ø±ÙˆØ¶
                if (!_user.IsAdmin)
                {
                    if (_previewAccountId is null)
                    {
                        MessageBox.Show("Ù…Ù† ÙØ¶Ù„Ùƒ Ø§Ø¶ØºØ· Request Ø£ÙˆÙ„Ù‹Ø§ Ù„Ø¹Ø±Ø¶ Ø§Ù„Ø­Ø³Ø§Ø¨ Ù‚Ø¨Ù„ Ø§Ù„Ø¨ÙŠØ¹.", "Sell", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // ØªØ­Ù‚Ù‘Ù‚ Ø¥Ù† Ù†ÙØ³ Ø§Ù„Ø­Ø³Ø§Ø¨ Ù…Ø§Ø²Ø§Ù„ Ø§Ù„Ù…Ø®ØªØ§Ø± Ø­Ø³Ø¨ Ø§Ù„Ù‚ÙˆØ§Ø¹Ø¯ Ø§Ù„Ø¢Ù†
                    var pickNow = await _rules.PickAccountAsync(game!, platform!.Value, type!.Value, _user.BranchId);
                    if (!pickNow.ok || pickNow.account is null)
                    {
                        MessageBox.Show(pickNow.message ?? "Ù„Ø§ ÙŠÙˆØ¬Ø¯ Ø­Ø³Ø§Ø¨ Ù…Ù†Ø§Ø³Ø¨ Ø­Ø§Ù„ÙŠÙ‹Ø§.", "Sell", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (pickNow.account.Id != _previewAccountId.Value)
                    {
                        MessageBox.Show(
                            "Ø§Ù„Ø­Ø³Ø§Ø¨ Ø§Ù„Ù…ØªØ§Ø­ ØªØºÙŠÙ‘Ø± Ù…Ù†Ø° Ø§Ù„Ù…Ø¹Ø§ÙŠÙ†Ø©. Ø£Ø¹Ø¯ Ø§Ù„Ø¶ØºØ· Ø¹Ù„Ù‰ Request Ø«Ù… Ø­Ø§ÙˆÙ„ Ø§Ù„Ø¨ÙŠØ¹.",
                            "Sell",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return;
                    }
                }

                // Ø¥Ù†Ø´Ø§Ø¡/Ø§Ù„Ø­ØµÙˆÙ„ Ø¹Ù„Ù‰ Ø¹Ù…ÙŠÙ„
                var customerId = await EnsureCustomerAsync(
                    _txtCustName.Text?.Trim(),
                    _txtCustPhone.Text?.Trim(),
                    _txtCustAddr.Text?.Trim()
                );

                var res = await _saleManager.SellAsync(new(
                    GameName: game!,
                    Platform: platform!.Value,
                    CopyType: type!.Value,
                    Price: (decimal)_numPrice.Value,
                    EmployeeUserId: _user.UserId,
                    CustomerId: customerId,
                    BranchId: _user.BranchId
                ));

                _log.AppendText($"{DateTime.Now}: {res.Message}. (Success={res.Success}){Environment.NewLine}");

                if (res.Success)
                {
                    // Ø¨Ø¹Ø¯ Ø¨ÙŠØ¹ Ù†Ø§Ø¬Ø­ Ù†ÙØ¶Ù‘ÙŠ Ø§Ù„Ù…Ø¹Ø§ÙŠÙ†Ø© Ù„Ø¥Ø¬Ø¨Ø§Ø± Ø£ÙŠ Ø¹Ù…Ù„ÙŠØ© Ø¬Ø¯ÙŠØ¯Ø© Ø¹Ù„Ù‰ Request
                    ClearPreview();
                    _txtCustName.Clear(); _txtCustPhone.Clear(); _txtCustAddr.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sell error:\n" + ex, "Sell", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

