using System.Windows.Forms;

namespace SonyERP.WinForms.Forms
{
    public partial class AccountsForm : Form
    {
        private Button btnPurchase;

        private void InitializeComponent()
        {
            this.btnPurchase = new Button();
            this.SuspendLayout();
            // 
            // btnPurchase
            // 
            this.btnPurchase.Name = "btnPurchase";
            this.btnPurchase.Text = "شراء حساب جديد";
            this.btnPurchase.AutoSize = true;
            this.btnPurchase.Location = new System.Drawing.Point(12, 12);
            // 
            // AccountsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnPurchase);
            this.Name = "AccountsForm";
            this.Text = "Accounts";
            this.ResumeLayout(false);
        }
    }
}
