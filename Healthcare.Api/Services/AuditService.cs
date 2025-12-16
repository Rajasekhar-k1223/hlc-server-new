using Healthcare.Api.Data;
using Healthcare.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Healthcare.Api.Services
{
    public interface IAuditService
    {
        Task LogAsync(string action, string details, string? userId = null, string? ipAddress = null);
    }

    public class AuditService : IAuditService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public AuditService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task LogAsync(string action, string details, string? userId = null, string? ipAddress = null)
        {
            // Use a new scope to avoid issues with concurrent contexts if called from async flows
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                
                var log = new AuditLog
                {
                    Action = action,
                    Details = details,
                    UserId = userId,
                    IpAddress = ipAddress,
                    Timestamp = DateTime.UtcNow
                };

                dbContext.AuditLogs.Add(log);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
