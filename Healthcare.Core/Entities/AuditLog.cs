using System;

namespace Healthcare.Core.Entities
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string Action { get; set; } // e.g., "Login", "RiskAnalysis"
        public string? UserId { get; set; } // Nullable because some actions might be pre-login
        public string Details { get; set; } // JSON details or summary
        public string? IpAddress { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
