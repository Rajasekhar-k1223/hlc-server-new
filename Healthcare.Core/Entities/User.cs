using System.ComponentModel.DataAnnotations;

namespace Healthcare.Core.Entities
{
    public class User : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.Patient;

        public Guid? OrganizationId { get; set; }
        public Organization? Organization { get; set; }
    }
}
