// Path: SonyERP.WinForms/forms/AccountsForm.cs
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using SonyERP.Forms.Dialogs; // Ù„Ø´Ø±Ø§Ø¡ Ø­Ø³Ø§Ø¨ Ø¬Ø¯ÙŠØ¯ Ø¹Ø¨Ø± Ø§Ù„Ù€Dialog

// Ù…Ù„Ø§Ø­Ø¸Ø©: Ù„Ùˆ Ø§Ù„Ù€Designer Ø¹Ù†Ø¯Ùƒ Ø¨ÙŠØ³ØªØ®Ø¯Ù… namespace Ù…Ø®ØªÙ„Ù ØºÙŠØ± "SonyERP.WinForms.forms"
// ØºÙŠÙ‘Ø± Ø§Ù„Ø³Ø·Ø± Ø§Ù„ØªØ§Ù„ÙŠ Ù„ÙŠØ·Ø§Ø¨Ù‚ Ø§Ù„Ù€Designer (Ø§ÙØªØ­ AccountsForm.Designer.cs ÙˆØªØ£ÙƒØ¯).
namespace SonyERP.WinForms.Forms
{
    public partial class AccountsForm : Form
    {
        // Ø²Ø± Ø§Ù„Ø´Ø±Ø§Ø¡ Ù„Ø§Ø²Ù… ÙŠÙƒÙˆÙ† Ù…Ø¹Ø±Ù Ø¯Ø§Ø®Ù„ Ø§Ù„ÙƒÙ„Ø§Ø³
        private Button? btnPurchase;

        // Ø§Ù„Ù€Constructor
        public AccountsForm()
        {
        InitializeComponent();
        BuildPurchaseButton();
            InitializeComponent();
            BuildPurchaseButton();
        }

        // Ø¥Ù†Ø´Ø§Ø¡ Ø²Ø± "Ø´Ø±Ø§Ø¡ Ø­Ø³Ø§Ø¨ Ø¬Ø¯ÙŠØ¯" ÙˆØ¥Ø¶Ø§ÙØªÙ‡ Ù„Ù„ÙÙˆØ±Ù…
        private void BuildPurchaseButton()
        {
            btnPurchase.Text = "Ø´Ø±Ø§Ø¡ Ø­Ø³Ø§Ø¨ Ø¬Ø¯ÙŠØ¯";
            btnPurchase.AutoSize = true;

            // Ø§Ø®ØªÙŠØ§Ø±ÙŠ: Ø¶Ø¹Ù‡ ÙÙŠ Ù…ÙƒØ§Ù† Ù…Ù†Ø§Ø³Ø¨ Ù„Ùˆ Ù…Ø§ÙÙŠØ´ Layout Manager
            btnPurchase.Left = 10;
            btnPurchase.Top  = 10;

            btnPurchase.Click += async (s, e) =>
            {
                using var dlg = new PurchaseAccountDialog(Program.ServiceProvider ?? throw new InvalidOperationException("DI not initialized."));
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    await LoadAccountsAsync(); // Ø§Ø³ØªØ¯Ø¹ Ù†ÙØ³ Ø·Ø±ÙŠÙ‚ØªÙƒ Ù„ØªØ­Ø¯ÙŠØ« Ø§Ù„Ø¬Ø±ÙŠØ¯
                    MessageBox.Show("ØªÙ… Ø¥Ø¶Ø§ÙØ© Ø§Ù„Ø­Ø³Ø§Ø¨ Ù„Ù„Ù…Ø®Ø²ÙˆÙ†.", "ØªÙ…",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };

            Controls.Add(btnPurchase);
        }

        // Ù„Ùˆ Ø¹Ù†Ø¯Ùƒ Ø¨Ø§Ù„ÙØ¹Ù„ Ø¯Ø§Ù„Ø© ØªØ­Ù…ÙŠÙ„ Ø§Ù„Ø­Ø³Ø§Ø¨Ø§Øª ÙÙŠ Partial Ø¢Ø®Ø±ØŒ Ø³ÙŠØ¨Ù‡Ø§ Ù‡Ù†Ø§Ùƒ.
        // Ø¥Ù† Ù…Ø§ÙƒØ§Ù†ØªØ´ Ù…ÙˆØ¬ÙˆØ¯Ø©ØŒ ÙØ¹Ù‘Ù„ Ø§Ù„Ù€Placeholder Ø§Ù„ØªØ§Ù„ÙŠ Ù…Ø¤Ù‚ØªÙ‹Ø§:
        //
        // private Task LoadAccountsAsync()
        // {
        //     // TODO: Ø­Ù…Ù‘Ù„ Ø§Ù„Ø¨ÙŠØ§Ù†Ø§Øª ÙˆØ§Ù…Ù„Ø£ Ø§Ù„Ø¬Ø±ÙŠØ¯
        //     return Task.CompletedTask;
        // }
    }
}





