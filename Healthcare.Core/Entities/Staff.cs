using System;
using System.ComponentModel.DataAnnotations;

namespace Healthcare.Core.Entities
{
    public class Staff : BaseEntity
    {
        [Required]
        public Guid UserId { get; set; } // Link to Authentication User
        
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string RequestEmail { get; set; } = string.Empty; // Temporary for onboarding

        public string Role { get; set; } = "Provider"; // Provider, Clinical, Admin, Pharmacy

        public string Department { get; set; } = "General"; // Cardiology, ER, etc.

        public string Status { get; set; } = "Active"; // Active, On Leave, Inactive

        public string Shift { get; set; } = "Day"; // Day, Night, Rotating
    }
}
