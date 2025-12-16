using System;
using System.ComponentModel.DataAnnotations;

namespace Healthcare.Core.Entities
{
    public class Appointment : BaseEntity
    {
        [Required]
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty; // Denormalized for easier display

        [Required]
        public Guid ProviderId { get; set; }
        public string ProviderName { get; set; } = string.Empty; // Denormalized

        [Required]
        public DateTime AppointmentDate { get; set; }

        public int DurationMinutes { get; set; } = 30;

        public string AppointmentType { get; set; } = "Checkup"; // Checkup, Follow-up, Emergency

        public string Status { get; set; } = "Scheduled"; // Scheduled, Checked-In, Completed, Cancelled

        public string? Reason { get; set; }
        public string? Notes { get; set; }
    }
}
