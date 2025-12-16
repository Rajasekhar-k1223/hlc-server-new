using Healthcare.Api.Data;
using Healthcare.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Healthcare.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiagnosticsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DiagnosticsController(AppDbContext context)
        {
            _context = context;
        }

        // --- Labs ---

        [HttpGet("labs")]
        public async Task<ActionResult<IEnumerable<LabRequest>>> GetLabRequests([FromQuery] string? status)
        {
            var query = _context.LabRequests.AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                // Mapping frontend status to backend status if needed, or ensuring alignment
                // Frontend: "pending", "completed" (lowercase)
                // Backend: "Pending", "Completed" (Title Case)
                var s = char.ToUpper(status[0]) + status.Substring(1); 
                query = query.Where(r => r.Status == s);
            }

            return await query.OrderByDescending(r => r.RequestedAt).ToListAsync();
        }

        [HttpPost("labs")]
        public async Task<ActionResult<LabRequest>> CreateLabRequest(LabRequest request)
        {
            _context.LabRequests.Add(request);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetLabRequests", new { id = request.Id }, request);
        }

        [HttpPut("labs/{id}")]
        public async Task<IActionResult> UpdateLabResult(Guid id, [FromBody] LabRequest update)
        {
            var request = await _context.LabRequests.FindAsync(id);
            if (request == null) return NotFound();

            if (!string.IsNullOrEmpty(update.Status)) request.Status = update.Status;
            if (!string.IsNullOrEmpty(update.ResultSummary)) request.ResultSummary = update.ResultSummary;
            if (update.Status == "Completed" && request.CompletedAt == null) request.CompletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(request);
        }

        // --- Imaging (PACS) ---

        [HttpGet("imaging")]
        public async Task<ActionResult<IEnumerable<ImagingRequest>>> GetImagingRequests()
        {
            return await _context.ImagingRequests.OrderByDescending(r => r.RequestedAt).ToListAsync();
        }

        [HttpPost("imaging")]
        public async Task<ActionResult<ImagingRequest>> CreateImagingRequest(ImagingRequest request)
        {
            // Assign mock image if empty for demo
            if (string.IsNullOrEmpty(request.ImageUrl))
            {
                request.ImageUrl = "https://prod-images-static.radiopaedia.org/images/1371286/9ba598007a51834927b233939f6004_jumbo.jpg";
            }
            
            _context.ImagingRequests.Add(request);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetImagingRequests", new { id = request.Id }, request);
        }
    }
}
