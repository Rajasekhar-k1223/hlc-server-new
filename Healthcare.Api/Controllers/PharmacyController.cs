using Healthcare.Api.Data;
using Healthcare.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Healthcare.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PharmacyController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PharmacyController(AppDbContext context)
        {
            _context = context;
        }

        // --- Inventory ---

        [HttpGet("inventory")]
        public async Task<ActionResult<IEnumerable<Medication>>> GetInventory([FromQuery] string? search)
        {
            var query = _context.Medications.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                var s = search.ToLower();
                query = query.Where(m => m.Name.ToLower().Contains(s) || m.Category.ToLower().Contains(s));
            }

            return await query.OrderBy(m => m.Name).ToListAsync();
        }

        [HttpPost("inventory")]
        public async Task<ActionResult<Medication>> AddInventory(Medication medication)
        {
            _context.Medications.Add(medication);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetInventory", new { id = medication.Id }, medication);
        }

        [HttpPut("inventory/{id}")]
        public async Task<IActionResult> UpdateInventory(Guid id, Medication medication)
        {
            if (id != medication.Id) return BadRequest();
            _context.Entry(medication).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // --- Prescriptions ---

        [HttpGet("prescriptions")]
        public async Task<ActionResult<IEnumerable<Prescription>>> GetPrescriptions([FromQuery] string? status)
        {
            var query = _context.Prescriptions.AsQueryable();
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(p => p.Status == status);
            }
            return await query.OrderByDescending(p => p.CreatedAt).ToListAsync();
        }

        [HttpPost("prescriptions")]
        public async Task<ActionResult<Prescription>> CreatePrescription(Prescription prescription)
        {
            _context.Prescriptions.Add(prescription);
            await _context.SaveChangesAsync();
            return Ok(prescription);
        }

        [HttpPost("dispense/{id}")]
        public async Task<IActionResult> DispensePrescription(Guid id)
        {
            var prescription = await _context.Prescriptions.FindAsync(id);
            if (prescription == null) return NotFound("Prescription not found");
            
            if (prescription.Status == "Dispensed") return BadRequest("Already dispensed");

            // Find matching medication to reduce stock
            // Weak link matching by name for now, simpler for this stage
            var med = await _context.Medications.FirstOrDefaultAsync(m => m.Name == prescription.MedicationName);
            if (med != null)
            {
                med.StockQuantity -= 1; // Assuming 1 unit per dispense for demo
                if (med.StockQuantity < 0) med.StockQuantity = 0;
            }

            prescription.Status = "Dispensed";
            prescription.DispensedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Dispensed successfully", RemainingStock = med?.StockQuantity ?? 0 });
        }
    }
}
