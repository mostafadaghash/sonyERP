using System.ComponentModel.DataAnnotations;

namespace SonyERP.Models
{
    public class Branch
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = default!;
    }
}
