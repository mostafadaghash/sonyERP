using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using SonyERP.Business;   // ICurrentUserContext
using SonyERP.Data;
using SonyERP.Models;     // Platform, CopyType

namespace SonyERP.WinForms.Forms
{
    public class DashboardForm : Form
    {
        private readonly ApplicationDbContext _db;
        private readonly ICurrentUserContext _user;

        // KPIs
        private readonly Label _kpiSales   = new() { AutoSize = true, Font = new Font("Segoe UI", 20, FontStyle.Bold) };
        private readonly Label _kpiRevenue = new() { AutoSize = true, Font = new Font("Segoe UI", 20, FontStyle.Bold) };
        private readonly Label _kpiAvg     = new() { AutoSize = true, Font = new Font("Segoe UI", 20, FontStyle.Bold) };

        // Stock summary (بشكل مبسط الآن)
        private readonly ProgressBar _pbPs4 = new() { Height = 22, Width = 280, Minimum = 0 };
        private readonly ProgressBar _pbPs5 = new() { Height = 22, Width = 280, Minimum = 0 };
        private readonly Label _lblPs4 = new() { AutoSize = true };
        private readonly Label _lblPs5 = new() { AutoSize = true };

        // Alerts (استحقاق 6 أشهر خلال 30 يومًا)
        private readonly DataGridView _gridAlerts = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = true };
        private readonly Button _btnRefresh = new() { Text = "تحديث", AutoSize = true };
        private readonly System.Windows.Forms.Timer _autoRefresh = new() { Interval = 60_000 }; // كل دقيقة

        public DashboardForm(ApplicationDbContext db, ICurrentUserContext user)
        {
            _db = db; _user = user;
            Text = "لوحة التحكم (Dashboard)";

            // Layout root
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 4, ColumnCount = 1, Padding = new Padding(10) };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));     // KPIs
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));     // Stock
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));     // Toolbar
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Alerts Grid

            // KPIs row
            var kpiRow = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true };
            kpiRow.Controls.Add(new Label { Text = "مبيعات اليوم:", AutoSize = true, Padding = new Padding(0, 8, 8, 0) });
            kpiRow.Controls.Add(_kpiSales);
            kpiRow.Controls.Add(new Label { Text = " | إيراد اليوم:", AutoSize = true, Padding = new Padding(12, 8, 8, 0) });
            kpiRow.Controls.Add(_kpiRevenue);
            kpiRow.Controls.Add(new Label { Text = " | متوسط السعر:", AutoSize = true, Padding = new Padding(12, 8, 8, 0) });
            kpiRow.Controls.Add(_kpiAvg);

            // Stock row
            var stockRow = new TableLayoutPanel { Dock = DockStyle.Top, ColumnCount = 3, AutoSize = true, Padding = new Padding(0, 10, 0, 0) };
            stockRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            stockRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            stockRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            var stockPs4Panel = new FlowLayoutPanel { AutoSize = true };
            stockPs4Panel.Controls.Add(new Label { Text = "PS4 Available:", AutoSize = true, Padding = new Padding(0, 2, 8, 0) });
            stockPs4Panel.Controls.Add(_pbPs4);
            stockPs4Panel.Controls.Add(_lblPs4);

            var stockPs5Panel = new FlowLayoutPanel { AutoSize = true, Padding = new Padding(20, 0, 0, 0) };
            stockPs5Panel.Controls.Add(new Label { Text = "PS5 Available:", AutoSize = true, Padding = new Padding(0, 2, 8, 0) });
            stockPs5Panel.Controls.Add(_pbPs5);
            stockPs5Panel.Controls.Add(_lblPs5);

            stockRow.Controls.Add(stockPs4Panel, 0, 0);
            stockRow.Controls.Add(stockPs5Panel, 1, 0);

            // Toolbar
            var toolbar = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, FlowDirection = FlowDirection.LeftToRight };
            _btnRefresh.Click += async (_, __) => await LoadDataAsync();
            toolbar.Controls.Add(_btnRefresh);
            toolbar.Controls.Add(new Label { Text = "تجديد تلقائي كل دقيقة", AutoSize = true, ForeColor = Color.DimGray, Padding = new Padding(12, 6, 0, 0) });

            // Alerts Group
            var alertsBox = new GroupBox { Text = "تنبيهات الاستحقاق (خلال 30 يومًا)", Dock = DockStyle.Fill, Padding = new Padding(8) };
            alertsBox.Controls.Add(_gridAlerts);

            // compose
            root.Controls.Add(kpiRow, 0, 0);
            root.Controls.Add(stockRow, 0, 1);
            root.Controls.Add(toolbar, 0, 2);
            root.Controls.Add(alertsBox, 0, 3);
            Controls.Add(root);

            // auto refresh
            _autoRefresh.Tick += async (_, __) => await LoadDataAsync();
            _autoRefresh.Start();

            Shown += async (_, __) => await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try { await LoadKpisAsync(); }
            catch (Exception ex) { MessageBox.Show("KPI error:\n" + ex.Message, "Dashboard", MessageBoxButtons.OK, MessageBoxIcon.Error); }

            try { await LoadStockAsync(); }
            catch (Exception ex) { MessageBox.Show("Stock error:\n" + ex.Message, "Dashboard", MessageBoxButtons.OK, MessageBoxIcon.Error); }

            try { await LoadAlertsAsync(); }
            catch (Exception ex) { MessageBox.Show("Alerts error:\n" + ex.Message, "Dashboard", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private async Task LoadKpisAsync()
        {
            // نفترض التخزين UTC
            var startUtc = DateTime.UtcNow.Date;    // بداية اليوم
            var endUtc   = startUtc.AddDays(1);     // بداية الغد

            var qToday = _db.Sales
                .AsNoTracking()
                .Where(s => s.BranchId == _user.BranchId
                            && s.SoldAt >= startUtc
                            && s.SoldAt < endUtc);

            var count   = await qToday.CountAsync();
            var revenue = await qToday.SumAsync(s => (decimal?)s.Price) ?? 0m;
            var avg     = count > 0 ? revenue / count : 0m;

            _kpiSales.Text   = count.ToString();
            _kpiRevenue.Text = revenue.ToString("0.##");
            _kpiAvg.Text     = avg.ToString("0.##");
        }

        private async Task LoadStockAsync()
        {
            // المخزون المتاح لكل منصة (على قد البيانات الحالية: IsAvailable فقط)
            var ps4Avail = await _db.GameAccounts
                .AsNoTracking()
                .Where(a => a.BranchId == _user.BranchId && a.Platform == Platform.PS4 && a.IsAvailable)
                .CountAsync();

            var ps5Avail = await _db.GameAccounts
                .AsNoTracking()
                .Where(a => a.BranchId == _user.BranchId && a.Platform == Platform.PS5 && a.IsAvailable)
                .CountAsync();

            var max = Math.Max(1, Math.Max(ps4Avail, ps5Avail));
            _pbPs4.Maximum = max;
            _pbPs5.Maximum = max;
            _pbPs4.Value = Math.Min(ps4Avail, _pbPs4.Maximum);
            _pbPs5.Value = Math.Min(ps5Avail, _pbPs5.Maximum);

            _lblPs4.Text = $"({ps4Avail})";
            _lblPs5.Text = $"({ps5Avail})";
        }

        private async Task LoadAlertsAsync()
        {
            // أي عملية تاريخها بين: (الآن − ٦ شهور) و (الآن − ٦ شهور + ٣٠ يوم)
            // معناها DueDate = SoldAt + 6 أشهر يقع خلال 30 يومًا القادمة.
            var nowUtc = DateTime.UtcNow;
            var startSoldAt = nowUtc.AddMonths(-6).Date;   // بداية نافذة السحب
            var endSoldAt   = startSoldAt.AddDays(30);     // نهاية النافذة

            var alertsQuery = _db.Sales
                .AsNoTracking()
                .Include(s => s.GameAccount)
                .Include(s => s.Customer)
                .Where(s => s.BranchId == _user.BranchId
                            && s.SoldAt >= startSoldAt
                            && s.SoldAt < endSoldAt)
                .Select(s => new
                {
                    s.Id,
                    Game = s.GameAccount != null ? s.GameAccount.GameName : "",
                    Platform = s.GameAccount != null ? s.GameAccount.Platform : Platform.PS5,
                    s.CopyType,
                    s.Price,
                    s.SoldAt,
                    Customer = s.Customer != null ? s.Customer.Name : null,
                    Phone = s.Customer != null ? s.Customer.Phone : null,
                });

            var raw = await alertsQuery
                .OrderBy(s => s.SoldAt)
                .ToListAsync();

            // حساب DueDate و DaysLeft خارج SQL لضمان التوافق مع SQLite
            var alerts = raw
                .Select(s =>
                {
                    var due = s.SoldAt.AddMonths(6);
                    var daysLeft = (int)Math.Ceiling((due - nowUtc).TotalDays);
                    return new
                    {
                        s.Id,
                        s.Game,
                        s.Platform,
                        s.CopyType,
                        s.Price,
                        SoldAt = s.SoldAt,
                        DueDate = due,
                        DaysLeft = daysLeft,
                        s.Customer,
                        s.Phone
                    };
                })
                .Where(x => x.DueDate >= nowUtc && x.DueDate < nowUtc.AddDays(30))
                .OrderBy(x => x.DueDate)
                .ToList();

            _gridAlerts.DataSource = alerts;
            ApplyAlertsGridFormatting();
        }

        // تنسيق آمن للجدول يمنع تحذيرات CS8602
        private void ApplyAlertsGridFormatting()
        {
            _gridAlerts.AllowUserToAddRows = false;
            _gridAlerts.AllowUserToDeleteRows = false;
            _gridAlerts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _gridAlerts.MultiSelect = false;
            _gridAlerts.ReadOnly = true;

            void fmt(string name, Action<DataGridViewColumn> action)
            {
                var col = _gridAlerts.Columns[name];
                if (col is not null) action(col);
            }

            fmt("Price",   c => c.DefaultCellStyle.Format = "0.##");
            fmt("SoldAt",  c => c.DefaultCellStyle.Format = "yyyy-MM-dd HH:mm");
            fmt("DueDate", c => c.DefaultCellStyle.Format = "yyyy-MM-dd");

            fmt("Platform", c => c.HeaderText = "منصّة");
            fmt("CopyType", c => c.HeaderText = "نوع النسخة");
            fmt("Customer", c => c.HeaderText = "العميل");
            fmt("Phone",    c => c.HeaderText = "الهاتف");

            fmt("DaysLeft", c =>
            {
                c.HeaderText = "الأيام المتبقية";
                c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            });

            // ترتيب الأعمدة (اختياري)
            fmt("DueDate", c => c.DisplayIndex = 0);
            fmt("DaysLeft", c => c.DisplayIndex = 1);
        }
    }
}
