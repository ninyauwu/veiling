using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Veiling.Server.Models;

namespace Veiling.Server.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class KavelsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public KavelsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/kavels
        [Authorize(Roles = 
        nameof(Role.Administrator) + ", " + 
        nameof(Role.Veilingmeester)
        )]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Kavel>>> GetKavels()
        {
            return await _context.Kavels
                .Include(k => k.Veiling)
                .Include(k => k.Leverancier)
                .Include(k => k.Boden)
                .ToListAsync();
        }

        // GET: api/kavels/5
        [Authorize(Roles = 
        nameof(Role.Administrator) + ", " + 
        nameof(Role.Veilingmeester)
        )]
        [HttpGet("{id}")]
        public async Task<ActionResult<Kavel>> GetKavel(int id)
        {
            var kavel = await _context.Kavels
                .Include(k => k.Veiling)
                .Include(k => k.Leverancier)
                .Include(k => k.Boden)
                .FirstOrDefaultAsync(k => k.Id == id);

            if (kavel == null)
            {
                return NotFound();
            }

            return kavel;
        }

        // GET: api/kavels/veiling/5
        [Authorize(Roles = 
        nameof(Role.Administrator) + ", " + 
        nameof(Role.Veilingmeester) + ", " + 
        nameof(Role.BedrijfManager) + ", " + 
        nameof(Role.Bedrijfsvertegenwoordiger) + ", " + 
        nameof(Role.Verkoper)
        )]
        [HttpGet("veiling/{veilingId}")]
        public async Task<ActionResult<IEnumerable<Kavel>>> GetKavelsByVeiling(int veilingId)
        {
            return await _context.Kavels
                .Where(k => k.VeilingId == veilingId)
                .Include(k => k.Leverancier)
                .Include(k => k.Boden)
                .ToListAsync();
        }

        // POST: api/kavels
        [Authorize(Roles = 
        nameof(Role.Administrator) + ", " + 
        nameof(Role.Verkoper)
        )]
        [HttpPost]
        public async Task<ActionResult<Kavel>> CreateKavel(Kavel kavel)
        {
            _context.Kavels.Add(kavel);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetKavel), new { id = kavel.Id }, kavel);
        }
//TODO: verkoper alleen zijn eigen kavels verranderen
        // PUT: api/kavels/5
        [Authorize(Roles = 
        nameof(Role.Administrator) + ", " + 
        nameof(Role.Veilingmeester) + ", " + 
        nameof(Role.Verkoper)
        )]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateKavel(int id, Kavel kavel)
        {
            if (id != kavel.Id)
            {
                return BadRequest();
            }

            _context.Entry(kavel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KavelExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/kavels/5
        [Authorize(Roles = 
        nameof(Role.Administrator) + ", " + 
        nameof(Role.Verkoper)
        )]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKavel(int id)
        {
            var kavel = await _context.Kavels.FindAsync(id);
            if (kavel == null)
            {
                return NotFound();
            }

            _context.Kavels.Remove(kavel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool KavelExists(int id)
        {
            return _context.Kavels.Any(k => k.Id == id);
        }
    }
}