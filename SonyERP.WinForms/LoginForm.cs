// Path: SonyERP.WinForms/LoginForm.cs
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using SonyERP.Business;
using SonyERP.Data;

namespace SonyERP.WinForms
{
    public class LoginForm : Form
    {
        private readonly ApplicationDbContext _db;
        private readonly LoggedInUserContext _ctx;

        private readonly TextBox _txtUser = new() { PlaceholderText = "Username", Dock = DockStyle.Top, Margin = new Padding(8) };
        private readonly TextBox _txtPass = new() { PlaceholderText = "Password", UseSystemPasswordChar = true, Dock = DockStyle.Top, Margin = new Padding(8) };
        private readonly Button _btnLogin = new() { Text = "Login", Dock = DockStyle.Top, Height = 40, Margin = new Padding(8) };
        private readonly Label _lblMsg = new() { Dock = DockStyle.Top, AutoSize = true, ForeColor = System.Drawing.Color.Firebrick, Margin = new Padding(8) };

        public LoginForm(ApplicationDbContext db, LoggedInUserContext ctx)
        {
            _db = db;
            _ctx = ctx;

            Text = "Login";
            StartPosition = FormStartPosition.CenterScreen;
            Width = 360; Height = 210;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(12) };
            panel.Controls.Add(_btnLogin);
            panel.Controls.Add(_txtPass);
            panel.Controls.Add(_txtUser);
            panel.Controls.Add(_lblMsg);
            Controls.Add(panel);

            _btnLogin.Click += async (_, __) => await DoLoginAsync();
            _txtPass.KeyDown += async (s, e) => { if (e.KeyCode == Keys.Enter) await DoLoginAsync(); };
            Shown += (_, __) => _txtUser.Focus();
        }

        private async Task DoLoginAsync()
        {
            try
            {
                _lblMsg.Text = "";
                var username = _txtUser.Text.Trim();
                var password = _txtPass.Text; // مبدئيًا Plain؛ لاحقًا نستخدم Hash/Verify

                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
                {
                    _lblMsg.Text = "من فضلك أدخل اسم المستخدم وكلمة المرور.";
                    return;
                }

                var user = await _db.Users
                    .Include(u => u.Branch)
                    .Where(u => u.Username == username)
                    .FirstOrDefaultAsync();

                if (user is null)
                {
                    _lblMsg.Text = "المستخدم غير موجود.";
                    return;
                }

                // مؤقتًا: مقارنة مباشرة مع PasswordHash المحفوظة
                if (!string.Equals(user.PasswordHash, password, StringComparison.Ordinal))
                {
                    _lblMsg.Text = "كلمة المرور غير صحيحة.";
                    return;
                }

                // ثبّت هوية المستخدم في الـ Context
                _ctx.Set(user.Id, user.BranchId, user.IsAdmin);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                _lblMsg.Text = "خطأ أثناء تسجيل الدخول.";
                MessageBox.Show("Login error:\n" + ex, "Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
