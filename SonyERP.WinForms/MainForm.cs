using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SonyERP.Business;          // ← مهم: فيه ICurrentUserContext
using SonyERP.Data;

namespace SonyERP.WinForms
{
    public class MainForm : Form
    {
        private readonly IServiceProvider _sp;
        private readonly ApplicationDbContext _db;
        private readonly ICurrentUserContext _user;

        // UI
        private readonly MenuStrip _menu = new();
        private readonly StatusStrip _status = new();
        private readonly ToolStripStatusLabel _lblUser = new();
        private readonly ToolStripStatusLabel _lblDb = new();
        private readonly ToolStripStatusLabel _lblClock = new() { Spring = true, TextAlign = ContentAlignment.MiddleRight };

        // حل التعارض: نحدد النوع كاملًا من WinForms
        private readonly System.Windows.Forms.Timer _clock = new() { Interval = 1000 };
        private int _dbTickCounter = 0;

        public MainForm(IServiceProvider sp, ApplicationDbContext db, ICurrentUserContext user)
        {
            _sp = sp;
            _db = db;
            _user = user;

            Text = "SonyERP — Main";
            IsMdiContainer = true;
            WindowState = FormWindowState.Maximized;

            BuildMenu();
            BuildStatus();

            Controls.Add(_menu);
            Controls.Add(_status);

            // ساعة + فحص اتصال DB دوري
            _clock.Tick += async (_, __) => await OnTickAsync();
            _clock.Start();

            // افتح لوحة التحكم افتراضيًا بعد الظهور
            Shown += (_, __) => OpenChild<Forms.DashboardForm>();
        }

        private void BuildMenu()
        {
            var mRoot = new ToolStripMenuItem("الرئيسية");
            var mOps  = new ToolStripMenuItem("العمليات");
            var mAdmin= new ToolStripMenuItem("الإدارة");
            var mRpt  = new ToolStripMenuItem("التقارير");
            var mSys  = new ToolStripMenuItem("النظام");

            // الرئيسية
            mRoot.DropDownItems.Add(new ToolStripMenuItem("لوحة التحكم (Dashboard)", null, (_, __) => OpenChild<Forms.DashboardForm>()));

            // العمليات
            mOps.DropDownItems.Add(new ToolStripMenuItem("بيع نسخة", null, (_, __) => OpenChild<Forms.SaleForm>()));
            mOps.DropDownItems.Add(new ToolStripMenuItem("استرجاع نسخة", null, (_, __) => OpenChild<Forms.ReclaimForm>()));

            // الإدارة (Admins only)
            mAdmin.DropDownItems.Add(new ToolStripMenuItem("إدارة الحسابات/المخزون", null, (_, __) => OpenChild<Forms.AccountsForm>()));
            mAdmin.DropDownItems.Add(new ToolStripMenuItem("إدارة العملاء", null, (_, __) => OpenChild<Forms.CustomersForm>()));

            // التقارير (Admins only)
            mRpt.DropDownItems.Add(new ToolStripMenuItem("التقارير (مجمعة/ربحية/استحقاقات)", null, (_, __) => OpenChild<Forms.ReportsForm>()));

            // النظام (Admins only)
            mSys.DropDownItems.Add(new ToolStripMenuItem("المستخدمون والصلاحيات", null, (_, __) => OpenChild<Forms.UsersForm>()));
            mSys.DropDownItems.Add(new ToolStripMenuItem("الإعدادات", null, (_, __) => OpenChild<Forms.SettingsForm>()));
            mSys.DropDownItems.Add(new ToolStripSeparator());
            mSys.DropDownItems.Add(new ToolStripMenuItem("تسجيل الخروج", null, (_, __) => Close()));

            // صلاحيات
            mAdmin.Enabled = _user.IsAdmin;
            mRpt.Enabled   = _user.IsAdmin;
            mSys.Enabled   = _user.IsAdmin;

            _menu.Items.AddRange(new[] { mRoot, mOps, mAdmin, mRpt, mSys });
            MainMenuStrip = _menu;
            _menu.Dock = DockStyle.Top;
        }

        private void BuildStatus()
        {
            _lblUser.Text = $"User: {_user.UserId} ({(_user.IsAdmin ? "Admin" : "Employee")})";
            _lblDb.Text = "DB: ...";
            _lblClock.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            _status.Items.Add(_lblUser);
            _status.Items.Add(new ToolStripStatusLabel(" | "));
            _status.Items.Add(_lblDb);
            _status.Items.Add(new ToolStripStatusLabel(" | "));
            _status.Items.Add(_lblClock);
            _status.Dock = DockStyle.Bottom;
        }

        private async Task OnTickAsync()
        {
            // الساعة كل ثانية
            _lblClock.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            // فحص DB كل 10 ثواني
            _dbTickCounter++;
            if (_dbTickCounter >= 10)
            {
                _dbTickCounter = 0;
                try
                {
                    var ok = await _db.Database.CanConnectAsync();
                    _lblDb.Text = ok ? "DB: متصل" : "DB: غير متصل";
                    _lblDb.ForeColor = ok ? Color.ForestGreen : Color.DarkRed;
                }
                catch
                {
                    _lblDb.Text = "DB: خطأ";
                    _lblDb.ForeColor = Color.DarkRed;
                }
            }
        }

        private void OpenChild<T>() where T : Form
        {
            // فعّل الموجود لو مفتوح
            foreach (var child in MdiChildren)
            {
                if (child is T)
                {
                    child.Activate();
                    return;
                }
            }

            // من الـ DI
            var frm = _sp.GetRequiredService<T>();
            frm.MdiParent = this;
            frm.WindowState = FormWindowState.Maximized;
            frm.Show();
        }
    }
}
