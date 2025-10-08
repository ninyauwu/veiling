using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Veiling.Server.Data;
using Veiling.Server.Models;

namespace Veiling.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VeilingenController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VeilingenController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/veilingen
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VeilingItem>>> GetVeilingen()  // 👈 Update!
        {
            return await _context.Veilingen
                .AsNoTracking()
                .ToListAsync();
        }

        // GET: api/veilingen/5
        [HttpGet("{id}")]
        public async Task<ActionResult<VeilingItem>> GetVeiling(int id)  // 👈 Update!
        {
            var veiling = await _context.Veilingen
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == id);

            if (veiling == null)
            {
                return NotFound();
            }

            return veiling;
        }

        // POST: api/veilingen
        [HttpPost]
        public async Task<ActionResult<VeilingItem>> CreateVeiling(VeilingItem veiling)  // 👈 Update!
        {
            _context.Veilingen.Add(veiling);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVeiling), new { id = veiling.Id }, veiling);
        }

        // PUT: api/veilingen/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVeiling(int id, VeilingItem veiling)  // 👈 Update!
        {
            if (id != veiling.Id)
            {
                return BadRequest();
            }

            _context.Entry(veiling).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VeilingExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/veilingen/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVeiling(int id)
        {
            var veiling = await _context.Veilingen.FindAsync(id);
            if (veiling == null)
            {
                return NotFound();
            }

            _context.Veilingen.Remove(veiling);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VeilingExists(int id)
        {
            return _context.Veilingen.Any(v => v.Id == id);
        }
    }
}