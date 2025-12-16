using Healthcare.Api.Data;
using Healthcare.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Healthcare.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BillingController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BillingController(AppDbContext context)
        {
            _context = context;
        }

        // --- Invoices ---

        [HttpGet("invoices")]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetInvoices([FromQuery] string? status)
        {
            var query = _context.Invoices.AsQueryable();

            if (!string.IsNullOrEmpty(status) && status != "All")
            {
                query = query.Where(i => i.Status == status);
            }

            return await query.OrderByDescending(i => i.CreatedAt).ToListAsync();
        }

        [HttpGet("invoices/{id}")]
        public async Task<ActionResult<Invoice>> GetInvoice(Guid id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null) return NotFound();
            return invoice;
        }

        [HttpPost("invoices")]
        public async Task<ActionResult<Invoice>> CreateInvoice(Invoice invoice)
        {
            // Set defaults if missing
            if (invoice.DueDate == default) invoice.DueDate = DateTime.UtcNow.AddDays(30);
            
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetInvoice", new { id = invoice.Id }, invoice);
        }

        [HttpPost("pay/{id}")]
        public async Task<IActionResult> MarkAsPaid(Guid id, [FromBody] string method)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null) return NotFound();

            if (invoice.Status == "Paid") return BadRequest("Already paid");

            invoice.Status = "Paid";
            invoice.PaymentMethod = method ?? "Cash";
            invoice.PaidAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Invoice paid successfully" });
        }

        // --- Stats ---
        
        [HttpGet("stats")]
        public async Task<ActionResult> GetStats()
        {
            var totalRevenue = await _context.Invoices.Where(i => i.Status == "Paid").SumAsync(i => i.Amount);
            var outstanding = await _context.Invoices.Where(i => i.Status != "Paid" && i.Status != "Cancelled").SumAsync(i => i.Amount);
            var pendingClaims = await _context.InsuranceClaims.CountAsync(c => c.Status == "Submitted");
            var dailyCount = await _context.Invoices.CountAsync(i => i.CreatedAt.Date == DateTime.UtcNow.Date);

            return Ok(new 
            { 
                TotalRevenue = totalRevenue, 
                Outstanding = outstanding, 
                PendingClaims = pendingClaims,
                DailyInvoices = dailyCount
            });
        }
    }
}
