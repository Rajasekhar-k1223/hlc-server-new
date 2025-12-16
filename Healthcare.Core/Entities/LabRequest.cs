using System;
using System.ComponentModel.DataAnnotations;

namespace Healthcare.Core.Entities
{
    public class LabRequest : BaseEntity
    {
        [Required]
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;

        [Required]
        public Guid ProviderId { get; set; }
        public string ProviderName { get; set; } = string.Empty;

        [Required]
        public string TestName { get; set; } = string.Empty; // e.g., "CBC", "Lipid Profile"

        public string Priority { get; set; } = "Routine"; // Routine, Urgent, Stat

        public string Status { get; set; } = "Pending"; // Pending, In Progress, Completed

        public string ResultSummary { get; set; } = string.Empty; // Brief text result
        public string SampleId { get; set; } = string.Empty; // Barcode/Label ID
        
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
    }
}
