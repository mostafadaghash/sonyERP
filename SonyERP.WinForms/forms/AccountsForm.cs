using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using SonyERP.Forms.Dialogs;

namespace SonyERP.WinForms.Forms
{
    public partial class AccountsForm : Form
    {
        public AccountsForm()
        {
            InitializeComponent();
            BuildPurchaseButton();
        }

        private void BuildPurchaseButton()
        {
            // btnPurchase مُعرّف في الـDesigner
            btnPurchase.Text = "شراء حساب جديد";
            btnPurchase.AutoSize = true;

            btnPurchase.Click += async (s, e) =>
            {
                var sp = Program.ServiceProvider ?? throw new InvalidOperationException("DI not initialized.");
                using var dlg = new PurchaseAccountDialog(sp);
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    await LoadAccountsAsync();
                    MessageBox.Show("تم إضافة الحساب للمخزون.", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };

            if (!this.Controls.Contains(btnPurchase))
                this.Controls.Add(btnPurchase);
        }

        private Task LoadAccountsAsync()
        {
            // TODO: تحميل الحسابات وتحديث الجريد
            return Task.CompletedTask;
        }
    }
}
