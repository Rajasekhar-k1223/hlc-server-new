using System.ComponentModel.DataAnnotations;

namespace Healthcare.Core.Entities
{
    public class Organization : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? Type { get; set; } // e.g., Provider, Payer, Supplier
        public string? Identifier { get; set; } // NPI or similar

        // Navigation
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
