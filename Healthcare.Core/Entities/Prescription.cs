using System;
using System.ComponentModel.DataAnnotations;

namespace Healthcare.Core.Entities
{
    public class Prescription : BaseEntity
    {
        [Required]
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;

        [Required]
        public Guid ProviderId { get; set; }
        public string ProviderName { get; set; } = string.Empty;

        public string MedicationName { get; set; } = string.Empty;
        public string Dosage { get; set; } = string.Empty; // e.g., "500mg"
        public string Frequency { get; set; } = string.Empty; // e.g., "Twice daily"
        
        public string Status { get; set; } = "Pending"; // Pending, Dispensed, Cancelled
        
        public DateTime? DispensedAt { get; set; }
    }
}
