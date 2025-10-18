using System.Collections.Generic;

namespace SonyERP.Models
{
    public class Supplier
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? ContactName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; } = true;

        public System.Collections.Generic.ICollection<EmailPurchase> EmailPurchases { get; set; }
            = new System.Collections.Generic.List<EmailPurchase>();
    }
}
