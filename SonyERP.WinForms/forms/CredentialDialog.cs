using System;
using System.Windows.Forms;
using SonyERP.Business;
using SonyERP.Business.Services;

namespace SonyERP.WinForms.Forms
{
    public class CredentialDialog : Form
    {
        private readonly IAuditService _audit;
        private readonly int _userId;
        private readonly int? _accountId;

        private readonly TextBox _txtEmail;
        private readonly TextBox _txtPassword;
        private readonly TextBox _txtTwoFa;

        private readonly CheckBox _chkShowPassword;
        private readonly CheckBox _chkShowTwoFa;

        private readonly Button _btnCopyEmail;
        private readonly Button _btnCopyPassword;
        private readonly Button _btnCopyTwoFa;

        private readonly Button _btnOk;
        private readonly Button _btnCancel;

        public CredentialDialog(
            IAuditService audit,
            int userId,
            int? accountId,
            string email,
            string password,
            string? twoFa)
        {
            _audit = audit;
            _userId = userId;
            _accountId = accountId;

            Text = "Account Credentials";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MinimizeBox = false;
            MaximizeBox = false;
            ShowInTaskbar = false;
            Width = 640;
            Height = 320;

            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 5, Padding = new Padding(10), AutoSize = true };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            // Email
            layout.Controls.Add(new Label { Text = "Email:", AutoSize = true, Padding = new Padding(0, 8, 8, 0) }, 0, 0);
            _txtEmail = new TextBox { ReadOnly = true, Text = email, Dock = DockStyle.Fill };
            layout.Controls.Add(_txtEmail, 1, 0);
            _btnCopyEmail = new Button { Text = "Copy", AutoSize = true };
            _btnCopyEmail.Click += async (_, __) => { Clipboard.SetText(_txtEmail.Text); await _audit.LogAsync("CopyEmail", _userId, _accountId, null, "User copied email"); };
            layout.Controls.Add(_btnCopyEmail, 2, 0);

            // Password
            layout.Controls.Add(new Label { Text = "Password:", AutoSize = true, Padding = new Padding(0, 8, 8, 0) }, 0, 1);
            _txtPassword = new TextBox { ReadOnly = true, UseSystemPasswordChar = true, Text = password, Dock = DockStyle.Fill };
            layout.Controls.Add(_txtPassword, 1, 1);
            _btnCopyPassword = new Button { Text = "Copy", AutoSize = true };
            _btnCopyPassword.Click += async (_, __) =>
            {
                Clipboard.SetText(password);
                await _audit.LogAsync("CopyPassword", _userId, _accountId, null, "User copied password");
            };
            layout.Controls.Add(_btnCopyPassword, 2, 1);

            _chkShowPassword = new CheckBox { Text = "Show", AutoSize = true };
            _chkShowPassword.CheckedChanged += async (_, __) =>
            {
                _txtPassword.UseSystemPasswordChar = !_chkShowPassword.Checked;
                if (_chkShowPassword.Checked)
                    await _audit.LogAsync("ShowPassword", _userId, _accountId, null, "User revealed password");
            };
            layout.Controls.Add(_chkShowPassword, 2, 2);

            // TwoFA (optional)
            layout.Controls.Add(new Label { Text = "2FA:", AutoSize = true, Padding = new Padding(0, 8, 8, 0) }, 0, 3);
            _txtTwoFa = new TextBox { ReadOnly = true, UseSystemPasswordChar = true, Text = string.IsNullOrWhiteSpace(twoFa) ? "(ØºÙŠØ± Ù…Ø³Ø¬Ù„)" : twoFa, Dock = DockStyle.Fill };
            layout.Controls.Add(_txtTwoFa, 1, 3);
            _btnCopyTwoFa = new Button { Text = "Copy", AutoSize = true };
            _btnCopyTwoFa.Click += async (_, __) =>
            {
                if (!string.IsNullOrWhiteSpace(twoFa))
                {
                    Clipboard.SetText(twoFa);
                    await _audit.LogAsync("Copy2FA", _userId, _accountId, null, "User copied 2FA");
                }
            };
            layout.Controls.Add(_btnCopyTwoFa, 2, 3);

            _chkShowTwoFa = new CheckBox { Text = "Show", AutoSize = true };
            _chkShowTwoFa.CheckedChanged += async (_, __) =>
            {
                _txtTwoFa.UseSystemPasswordChar = !_chkShowTwoFa.Checked;
                if (_chkShowTwoFa.Checked)
                    await _audit.LogAsync("Show2FA", _userId, _accountId, null, "User revealed 2FA");
            };
            layout.Controls.Add(_chkShowTwoFa, 2, 4);

            // Buttons
            var buttons = new FlowLayoutPanel { FlowDirection = FlowDirection.RightToLeft, Dock = DockStyle.Bottom, Height = 48 };
            _btnOk = new Button { Text = "OK", AutoSize = true, DialogResult = DialogResult.OK };
            _btnCancel = new Button { Text = "Cancel", AutoSize = true, DialogResult = DialogResult.Cancel };
            buttons.Controls.Add(_btnOk);
            buttons.Controls.Add(_btnCancel);

            var root = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1 };
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.Controls.Add(layout, 0, 0);
            root.Controls.Add(buttons, 0, 1);

            Controls.Add(root);

            AcceptButton = _btnOk;
            CancelButton = _btnCancel;

            // Ø£ÙˆÙ„ Ù…Ø§ ØªÙØªØ­ Ø§Ù„Ù†Ø§ÙØ°Ø© â€” Ù„Ø§ Ù†Ø³Ø¬Ù‘Ù„ Ø´ÙŠØ¦Ù‹Ø§. Ø§Ù„ØªØ³Ø¬ÙŠÙ„ ÙÙ‚Ø· Ø¹Ù†Ø¯ reveal/copy
        }
    }
}

