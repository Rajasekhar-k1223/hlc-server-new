using System;
using System.ComponentModel.DataAnnotations;

namespace Healthcare.Core.Entities
{
    public class Medication : BaseEntity
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        
        public string Category { get; set; } = "General"; // Antibiotic, Analgesic, etc.
        
        public int StockQuantity { get; set; }
        
        public string Unit { get; set; } = "Tablets"; // Tablets, Bottles, mL
        
        public int ReorderLevel { get; set; } = 10;
        
        public DateTime ExpiryDate { get; set; }
        
        public string Status => StockQuantity <= 0 ? "Out of Stock" : 
                              StockQuantity <= ReorderLevel ? "Low Stock" : "In Stock";
    }
}
