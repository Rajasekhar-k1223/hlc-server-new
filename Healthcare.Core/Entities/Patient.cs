using System.ComponentModel.DataAnnotations;

namespace Healthcare.Core.Entities
{
    public class Patient : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty; // Male, Female, Other, Unknown

        public string? Phone { get; set; }
        public string? Address { get; set; }

        // FHIR-like Identifier
        public string? MRN { get; set; } // Medical Record Number

        public Guid? ManagingOrganizationId { get; set; } // The primary provider org

        // Clinical Snapshot (Denormalized for list view)
        public string Condition { get; set; } = "Healthy Checkup";
        public string Status { get; set; } = "Stable"; // Stable, Critical, etc.
        public DateTime? LastVisit { get; set; }
        
        // Vitals Snapshot
        public string BloodPressure { get; set; } = "120/80";
        public int HeartRate { get; set; } = 70;
    }
}
