using Healthcare.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Healthcare.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("stats")]
        public async Task<ActionResult> GetStats()
        {
            var totalPatients = await _context.Patients.CountAsync();
            var criticalPatients = await _context.Patients.CountAsync(p => p.Status == "Critical");
            var documentsProcessed = await _context.LabRequests.CountAsync(r => r.ResultSummary != ""); // Proxy for processed docs
            
            // AI Accuracy - Mock for now as we don't have a live feedback loop for accuracy yet
            var aiAccuracy = 98.5; 

            // Weekly volume - Mock or aggregation
            var weeklyData = new [] 
            {
                new { name = "Mon", docs = 42 },
                new { name = "Tue", docs = 35 },
                new { name = "Wed", docs = 28 },
                new { name = "Thu", docs = 45 },
                new { name = "Fri", docs = 30 },
                new { name = "Sat", docs = 20 },
                new { name = "Sun", docs = 15 }
            };

            return Ok(new 
            {
                TotalPatients = totalPatients,
                CriticalAlerts = criticalPatients,
                DocumentsProcessed = documentsProcessed,
                AiAccuracy = aiAccuracy,
                WeeklyVolume = weeklyData
            });
        }
    }
}
