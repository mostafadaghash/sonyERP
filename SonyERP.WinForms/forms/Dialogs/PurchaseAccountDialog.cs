using SonyERP.Business;
using SonyERP.Models;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using SonyERP.Business.DTOs;
using SonyERP.Business.Services;
using SonyERP.DAL;
namespace SonyERP.Forms.Dialogs
{
    public class PurchaseAccountDialog : Form
    {
        private readonly IServiceProvider _sp;
        private ComboBox cmbSupplier = new ComboBox();
        private ComboBox cmbPlatform = new ComboBox();
        private TextBox txtGame = new TextBox();
        private TextBox txtEmail = new TextBox();
        private TextBox txtPassword = new TextBox();
        private TextBox txtTwoFactor = new TextBox();
        private NumericUpDown numCost = new NumericUpDown();
        private NumericUpDown numOffline = new NumericUpDown();
        private NumericUpDown numPrimary = new NumericUpDown();
        private NumericUpDown numSecondary = new NumericUpDown();
        private NumericUpDown numBranchId = new NumericUpDown();
        private TextBox txtNotes = new TextBox();
        private Button btnSave = new Button();

        public int? CreatedGameAccountId { get; private set; }

        public PurchaseAccountDialog(IServiceProvider sp)
        {
            _sp = sp;
            BuildUi();
            _ = LoadSuppliersAsync();
        }

        private void BuildUi()
        {
            Text = "Ø´Ø±Ø§Ø¡ Ø­Ø³Ø§Ø¨ Ø¬Ø¯ÙŠØ¯";
            Width = 520; Height = 600; StartPosition = FormStartPosition.CenterParent;

            var lblSupplier = new Label { Text = "Ø§Ù„Ù…ÙˆØ±Ø¯", Left = 20, Top = 20, Width = 120 };
            cmbSupplier.Left = 160; cmbSupplier.Top = 20; cmbSupplier.Width = 300;

            var lblGame = new Label { Text = "Ø§Ù„Ù„Ø¹Ø¨Ø©", Left = 20, Top = 60, Width = 120 };
            txtGame.Left = 160; txtGame.Top = 60; txtGame.Width = 300;

            var lblPlatform = new Label { Text = "Ø§Ù„Ù…Ù†ØµØ©", Left = 20, Top = 100, Width = 120 };
            cmbPlatform.Left = 160; cmbPlatform.Top = 100; cmbPlatform.Width = 300;
            cmbPlatform.DataSource = Enum.GetValues(typeof(Platform));

            var lblEmail = new Label { Text = "Email", Left = 20, Top = 140, Width = 120 };
            txtEmail.Left = 160; txtEmail.Top = 140; txtEmail.Width = 300;

            var lblPass = new Label { Text = "Password", Left = 20, Top = 180, Width = 120 };
            txtPassword.Left = 160; txtPassword.Top = 180; txtPassword.Width = 300;

            var lbl2fa = new Label { Text = "TwoFactorKey", Left = 20, Top = 220, Width = 120 };
            txtTwoFactor.Left = 160; txtTwoFactor.Top = 220; txtTwoFactor.Width = 300;

            var lblCost = new Label { Text = "Cost", Left = 20, Top = 260, Width = 120 };
            numCost.Left = 160; numCost.Top = 260; numCost.Width = 120; numCost.DecimalPlaces = 2; numCost.Maximum = 1000000;

            var lblOffline = new Label { Text = "Offline quota", Left = 20, Top = 300, Width = 120 };
            numOffline.Left = 160; numOffline.Top = 300; numOffline.Minimum = 0; numOffline.Maximum = 50; numOffline.Value = 2;

            var lblPrimary = new Label { Text = "Primary quota", Left = 20, Top = 340, Width = 120 };
            numPrimary.Left = 160; numPrimary.Top = 340; numPrimary.Minimum = 0; numPrimary.Maximum = 10; numPrimary.Value = 1;

            var lblSecondary = new Label { Text = "Secondary quota", Left = 20, Top = 380, Width = 120 };
            numSecondary.Left = 160; numSecondary.Top = 380; numSecondary.Minimum = 0; numSecondary.Maximum = 50;

            var lblBranch = new Label { Text = "BranchId", Left = 20, Top = 420, Width = 120 };
            numBranchId.Left = 160; numBranchId.Top = 420; numBranchId.Minimum = 1; numBranchId.Maximum = 9999; numBranchId.Value = 1;

            var lblNotes = new Label { Text = "Ù…Ù„Ø§Ø­Ø¸Ø§Øª", Left = 20, Top = 460, Width = 120 };
            txtNotes.Left = 160; txtNotes.Top = 460; txtNotes.Width = 300; txtNotes.Height = 40; txtNotes.Multiline = true;

            btnSave.Text = "Ø­ÙØ¸";
            btnSave.Left = 160; btnSave.Top = 520; btnSave.Width = 120;
            btnSave.Click += async (s, e) => await SaveAsync();

            Controls.AddRange(new Control[] {
                lblSupplier, cmbSupplier, lblGame, txtGame, lblPlatform, cmbPlatform,
                lblEmail, txtEmail, lblPass, txtPassword, lbl2fa, txtTwoFactor,
                lblCost, numCost, lblOffline, numOffline, lblPrimary, numPrimary,
                lblSecondary, numSecondary, lblBranch, numBranchId, lblNotes, txtNotes,
                btnSave
            });
        }

        private async Task LoadSuppliersAsync()
        {
            var repo = _sp.GetRequiredService<ISupplierRepository>();
            var suppliers = await repo.GetActiveAsync();
            cmbSupplier.DataSource = suppliers;
            cmbSupplier.DisplayMember = "Name";
            cmbSupplier.ValueMember = "Id";
        }

        private async Task SaveAsync()
        {
            try
            {
                var svc = _sp.GetRequiredService<PurchaseService>();
                var dto = new EmailPurchaseDTO
                {
                    SupplierId = Convert.ToInt32(cmbSupplier.SelectedValue ?? 0),
                    BranchId = Convert.ToInt32(numBranchId.Value),
                    GameName = txtGame.Text.Trim(),
                    Platform = (Platform)cmbPlatform.SelectedItem,
                    AccountEmail = txtEmail.Text.Trim(),
                    AccountPassword = txtPassword.Text,
                    TwoFactorKey = string.IsNullOrWhiteSpace(txtTwoFactor.Text) ? null : txtTwoFactor.Text.Trim(),
                    Cost = numCost.Value,
                    InitialOfflineQuota = Convert.ToInt32(numOffline.Value),
                    InitialPrimaryQuota = Convert.ToInt32(numPrimary.Value),
                    InitialSecondaryQuota = Convert.ToInt32(numSecondary.Value),
                    Notes = string.IsNullOrWhiteSpace(txtNotes.Text) ? null : txtNotes.Text.Trim()
                };

                // TODO: Ø¨Ø¯Ù‘Ù„ Ø§Ù„Ù‚ÙŠÙ…Ø© Ø¨Ù…ØµØ¯Ø± Ø§Ù„Ø­Ù‚ÙŠÙ‚Ø© Ø¹Ù†Ø¯Ùƒ (CurrentUserContext)
                var currentUserId = 1;
                CreatedGameAccountId = await svc.PurchaseEmailAccountAsync(dto, currentUserId);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ÙØ´Ù„ Ø§Ù„Ø­ÙØ¸: {ex.Message}", "Ø®Ø·Ø£", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}



