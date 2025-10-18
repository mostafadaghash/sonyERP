using Microsoft.EntityFrameworkCore;
using SonyERP.Models;

namespace SonyERP.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // Entities
        public DbSet<GameAccount> GameAccounts => Set<GameAccount>();
        public DbSet<Sale>        Sales        => Set<Sale>();
        public DbSet<User>        Users        => Set<User>();
        public DbSet<Customer>    Customers    => Set<Customer>();
        public DbSet<Branch>      Branches     => Set<Branch>();     // ← المطلوب
        public DbSet<AuditEvent> AuditEvents => Set<AuditEvent>();
        public DbSet<Supplier> Suppliers { get; set; } = null!;
        public DbSet<EmailPurchase> EmailPurchases { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // فهارس مفيدة
            modelBuilder.Entity<GameAccount>()
                .HasIndex(x => new { x.GameName, x.Platform, x.BranchId });

            modelBuilder.Entity<GameAccount>()
                .HasIndex(x => x.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<AuditEvent>()
                .HasIndex(x => x.TimestampUtc);
            modelBuilder.Entity<AuditEvent>()
                .HasIndex(x => x.Action);

            modelBuilder.Entity<AuditEvent>()
                .Property(x => x.Action).HasMaxLength(64);
            modelBuilder.Entity<AuditEvent>()
                .Property(x => x.Details).HasMaxLength(2000);
        }
    }
}
