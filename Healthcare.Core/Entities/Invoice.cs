using System;
using System.ComponentModel.DataAnnotations;

namespace Healthcare.Core.Entities
{
    public class Invoice : BaseEntity
    {
        [Required]
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;

        public string Description { get; set; } = "Medical Service";
        
        public decimal Amount { get; set; }
        
        public string Status { get; set; } = "Unpaid"; // Unpaid, Paid, Overdue, Cancelled
        
        public string PaymentMethod { get; set; } = "None"; // Credit Card, Insurance, Cash
        
        public DateTime DueDate { get; set; }
        public DateTime? PaidAt { get; set; }
    }
}
