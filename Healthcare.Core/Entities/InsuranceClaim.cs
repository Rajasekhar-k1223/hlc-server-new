using System;
using System.ComponentModel.DataAnnotations;

namespace Healthcare.Core.Entities
{
    public class InsuranceClaim : BaseEntity
    {
        [Required]
        public Guid InvoiceId { get; set; }
        
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        
        public string Provider { get; set; } = string.Empty; // e.g., BlueCross
        public string PolicyNumber { get; set; } = string.Empty;
        
        public decimal ClaimAmount { get; set; }
        
        public string Status { get; set; } = "Submitted"; // Submitted, Approved, Rejected, Paid
        
        public string? Notes { get; set; }
    }
}
