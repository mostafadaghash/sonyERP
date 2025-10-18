using System;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SonyERP.Business;
using SonyERP.Data;
using SonyERP.Business.Services;
using SonyERP.DAL;

namespace SonyERP.WinForms
{
    internal static class Program
    {
    public static IServiceProvider? ServiceProvider { get; set; }

        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                using var host = CreateHostBuilder().Build();

                // Ù…ÙŽÙŠÙ’Ø¬Ø±ÙŽÙŠÙ’Ø´Ù† + Seed Ù…Ø¹ Ø§Ù„ØªÙ‚Ø§Ø· Ø§Ù„Ø£Ø®Ø·Ø§Ø¡ Ù„Ø¹Ø±Ø¶Ù‡Ø§ ÙÙŠ MessageBox
                try
                {
                    using var scope = host.Services.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    db.Database.Migrate();

                    // Seed: ÙØ±Ø¹
                    if (!db.Branches.Any())
                    {
                        db.Branches.Add(new Models.Branch { Name = "Main" });
                        db.SaveChanges();
                    }

                    // Seed: ÙŠÙˆØ²Ø± Ø£Ø¯Ù…Ù†
                    if (!db.Users.Any(u => u.Username == "admin"))
                    {
                        db.Users.Add(new Models.User
                        {
                            Username = "admin",
                            PasswordHash = "x", // Ù…Ø¤Ù‚ØªÙ‹Ø§ â€“ Ù„Ø§Ø­Ù‚Ù‹Ø§ Hash
                            BranchId = 1,
                            IsAdmin = true
                        });
                        db.SaveChanges();
                    }

                    // Seed: Ù…ÙˆØ¸Ù Ù„Ù„ØªØ¬Ø±Ø¨Ø©
                    if (!db.Users.Any(u => u.Username == "employee"))
                    {
                        db.Users.Add(new Models.User
                        {
                            Username = "employee",
                            PasswordHash = "1234", // Ù…Ø¤Ù‚ØªÙ‹Ø§
                            BranchId = 1,
                            IsAdmin = false
                        });
                        db.SaveChanges();
                    }

                    // Seed: Ø¹Ù…ÙŠÙ„ ØªØ¬Ø±ÙŠØ¨ÙŠ
                    if (!db.Customers.Any())
                    {
                        db.Customers.Add(new Models.Customer
                        {
                            Name = "Ø¹Ù…ÙŠÙ„ ØªØ¬Ø±ÙŠØ¨ÙŠ",
                            Phone = "01000000000",
                            Address = "Ø§Ù„Ù‚Ø§Ù‡Ø±Ø©"
                        });
                        db.SaveChanges();
                    }

                    // Seed: Ø­Ø³Ø§Ø¨Ø§Øª Ø§Ø®ØªØ¨Ø§Ø± FC26/PS5
                    if (!db.GameAccounts.Any(a => a.GameName == "FC26" && a.Platform == Models.Platform.PS5 && a.BranchId == 1))
                    {
                        db.GameAccounts.AddRange(
                            new Models.GameAccount
                            {
                                GameName = "FC26",
                                Platform = Models.Platform.PS5,
                                Email = "acc1@mail.com",
                                Password = "p1",
                                BranchId = 1,
                                IsAvailable = true,
                                Ps5OfflineLeft = 2, Ps5SecondaryLeft = 1, Ps5PrimaryLeft = 1,
                                CreatedAt = DateTime.UtcNow.AddMinutes(-10)
                            },
                            new Models.GameAccount
                            {
                                GameName = "FC26",
                                Platform = Models.Platform.PS5,
                                Email = "acc2@mail.com",
                                Password = "p2",
                                BranchId = 1,
                                IsAvailable = true,
                                Ps5OfflineLeft = 2, Ps5SecondaryLeft = 1, Ps5PrimaryLeft = 1,
                                CreatedAt = DateTime.UtcNow.AddMinutes(-5)
                            }
                        );
                        db.SaveChanges();
                    }
                }
                catch (Exception migEx)
                {
                    MessageBox.Show(
                        "Migration/DB error:\n" + migEx,
                        "Startup Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    // Ù…Ù…ÙƒÙ† ØªÙ†Ù‡ÙŠ Ø§Ù„ØªØ´ØºÙŠÙ„ Ù‡Ù†Ø§ Ù„Ùˆ Ø­Ø¨ÙŠØª: return;
                }

                // Ø´Ø§Ø´Ø© Ø§Ù„Ø¯Ø®ÙˆÙ„ Ø£ÙˆÙ„Ù‹Ø§
                var login = host.Services.GetRequiredService<LoginForm>();
                if (login.ShowDialog() != DialogResult.OK)
                    return;

                // Ø«Ù… Ø§Ù„ÙˆØ§Ø¬Ù‡Ø© Ø§Ù„Ø±Ø¦ÙŠØ³ÙŠØ© (MDI)
                var main = host.Services.GetRequiredService<MainForm>();
                Application.Run(main);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Fatal startup error:\n" + ex,
                    "SonyERP.WinForms",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        static IHostBuilder CreateHostBuilder() =>
            Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((ctx, cfg) =>
                {
                    // Ù†Ù‚Ø±Ø£ appsettings.json Ù…Ù† Ù…Ø¬Ù„Ø¯ Ø§Ù„Ø¥Ø®Ø±Ø§Ø¬
                    cfg.SetBasePath(AppContext.BaseDirectory)
                       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureLogging(l =>
                {
                    l.ClearProviders();
                    l.AddDebug();
                })
                .ConfigureServices((ctx, services) =>
                {
                    // Database
                    var provider = ctx.Configuration["Database:Provider"] ?? "SQLite";
                    if (provider.Equals("PostgreSQL", StringComparison.OrdinalIgnoreCase))
                    {
                        services.AddDbContext<ApplicationDbContext>(o =>
                            o.UseNpgsql(ctx.Configuration["Database:PostgreSQL:ConnectionString"]));
                    }
                    else
                    {
                        var cs = ctx.Configuration["Database:SQLite:ConnectionString"] ?? "Data Source=sony_erp.db";
                        services.AddDbContext<ApplicationDbContext>(o => o.UseSqlite(cs));
                    }

                    // ===== DI: Data layer =====
                    // ØªØ£ÙƒØ¯Ù†Ø§ Ù…Ù† Ø§Ù„Ù€ namespace Ø§Ù„ØµØ­ÙŠØ­ Ù‡Ù†Ø§
                    services.AddScoped<SonyERP.Data.IUnitOfWork, SonyERP.Data.UnitOfWork>();

                    // ===== DI: Business layer =====
                    services.AddScoped<ISalesRulesService, SalesRulesService>();
                    services.AddScoped<IAuthorizationService, AuthorizationService>();
                    services.AddScoped<ISaleManager, SaleManager>();
                    services.AddScoped<IAuditService, AuditLogServiceDb>(); // ÙŠÙƒØªØ¨ ÙÙŠ AuditEvents
                    

                    // Ø³ÙŠØ§Ù‚ Ø§Ù„Ù…Ø³ØªØ®Ø¯Ù… Ø§Ù„Ø­Ù‚ÙŠÙ‚ÙŠ (Ø¨Ø¯Ù„ Dummy)
                    services.AddSingleton<LoggedInUserContext>();
                    services.AddSingleton<ICurrentUserContext>(sp => sp.GetRequiredService<LoggedInUserContext>());

                    // ===== UI Forms =====
                    // Ù…Ù„Ø§Ø­Ø¸Ø© Ù…Ù‡Ù…Ø©: Ù„Ø§ ØªØ³Ø¬Ù‘Ù„ CredentialDialog ÙÙŠ Ø§Ù„Ù€ DI Ù„Ø£Ù†Ù‡ ÙŠØ­ØªØ§Ø¬ userId (int)
                    services.AddTransient<LoginForm>();
                    services.AddTransient<MainForm>();
                    services.AddTransient<Forms.DashboardForm>();
                    services.AddTransient<Forms.SaleForm>();
                    services.AddTransient<Forms.ReclaimForm>();
                    services.AddTransient<Forms.AccountsForm>();
                    services.AddTransient<Forms.CustomersForm>();
                    services.AddTransient<Forms.ReportsForm>();
                    services.AddTransient<Forms.UsersForm>();
                    services.AddTransient<Forms.SettingsForm>();
                });
    }
}


