using Healthcare.Api.Data;
using Healthcare.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Healthcare.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrganizationController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrganizationController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Organization>>> GetOrganizations([FromQuery] string? type)
        {
            var query = _context.Organizations.AsQueryable();
            if (!string.IsNullOrEmpty(type) && type != "All")
            {
                query = query.Where(o => o.Type == type);
            }
            return await query.OrderBy(o => o.Name).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Organization>> GetOrganization(Guid id)
        {
            var org = await _context.Organizations.FindAsync(id);
            if (org == null) return NotFound();
            return org;
        }

        [HttpPost]
        public async Task<ActionResult<Organization>> CreateOrganization(Organization org)
        {
            if (string.IsNullOrEmpty(org.ImageUrl)) 
                org.ImageUrl = "https://images.unsplash.com/photo-1519494026892-80bbd2d6fd0d?auto=format&fit=crop&q=80"; // Default

            _context.Organizations.Add(org);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetOrganization", new { id = org.Id }, org);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrganization(Guid id, Organization org)
        {
            if (id != org.Id) return BadRequest();
            _context.Entry(org).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrganization(Guid id)
        {
             var org = await _context.Organizations.FindAsync(id);
             if (org == null) return NotFound();
             _context.Organizations.Remove(org);
             await _context.SaveChangesAsync();
             return NoContent();
        }
    }
}
