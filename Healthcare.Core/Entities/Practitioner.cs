using System.ComponentModel.DataAnnotations;

namespace Healthcare.Core.Entities
{
    public class Practitioner : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Specialty { get; set; }
        public string? NPI { get; set; } // National Provider Identifier

        public Guid? UserId { get; set; } // Link to login user if applicable
        public User? User { get; set; }
    }
}
