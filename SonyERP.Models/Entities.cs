using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SonyERP.Models;

public class User
{
    [Key] public int Id { get; set; }
    [MaxLength(100)] public string Username { get; set; } = "";
    [MaxLength(256)] public string PasswordHash { get; set; } = "";
    public int BranchId { get; set; }
    public Branch? Branch { get; set; }
    public bool IsAdmin { get; set; }
}

public class Customer
{
    [Key] public int Id { get; set; }
    [MaxLength(120)] public string Name { get; set; } = "";
    [MaxLength(30)]  public string? Phone { get; set; }
    [MaxLength(200)] public string? Address { get; set; }
    [MaxLength(120)] public string? Notes { get; set; }
}

public class Sale
{
    [Key] public int Id { get; set; }

    public int GameAccountId { get; set; }
    public GameAccount? GameAccount { get; set; }

    public CopyType CopyType { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal Price { get; set; }

    // مُنفّذ العملية والعميل والفرع
    public int EmployeeUserId { get; set; }
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public int BranchId { get; set; }

    public DateTime SoldAt { get; set; } = DateTime.UtcNow;
}

public class AuditLog
{
    [Key] public int Id { get; set; }

    [MaxLength(50)]  public string Action { get; set; } = ""; // e.g., "SELL", "VIEW_2FA", "EXCEPTION_PRIMARY_BEFORE_2OFFLINE"
    [MaxLength(50)]  public string Entity { get; set; } = ""; // e.g., "GameAccount", "Sale", "Customer"
    public int? EntityId { get; set; }

    public int UserId { get; set; }
    public int BranchId { get; set; }

    [MaxLength(1024)] public string? Details { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
