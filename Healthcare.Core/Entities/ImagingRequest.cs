using System;
using System.ComponentModel.DataAnnotations;

namespace Healthcare.Core.Entities
{
    public class ImagingRequest : BaseEntity
    {
        [Required]
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;

        [Required]
        public string Modality { get; set; } = "X-Ray"; // X-Ray, CT, MRI, Ultrasound
        
        public string BodyPart { get; set; } = string.Empty; // Chest, Knee, Head
        
        public string Indication { get; set; } = string.Empty; // Reason for scan

        public string Status { get; set; } = "Scheduled"; // Scheduled, Processing, Reported
        
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        
        public string ImageUrl { get; set; } = string.Empty; // Mock URL for PACS
    }
}
