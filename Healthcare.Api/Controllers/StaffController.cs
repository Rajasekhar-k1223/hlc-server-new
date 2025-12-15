using Healthcare.Api.Data;
using Healthcare.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Healthcare.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StaffController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StaffController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Staff>>> GetStaff()
        {
            return await _context.StaffMembers.OrderBy(s => s.Name).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Staff>> AddStaff(Staff staff)
        {
            // In a real app, this would also create a User Access login
            if (staff.UserId == Guid.Empty) staff.UserId = Guid.NewGuid(); // Mock ID for demo

            _context.StaffMembers.Add(staff);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetStaff", new { id = staff.Id }, staff);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStaff(Guid id, Staff staff)
        {
            if (id != staff.Id) return BadRequest();
            _context.Entry(staff).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveStaff(Guid id)
        {
            var staff = await _context.StaffMembers.FindAsync(id);
            if (staff == null) return NotFound();

            _context.StaffMembers.Remove(staff);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
