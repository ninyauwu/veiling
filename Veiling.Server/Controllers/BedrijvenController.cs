using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Veiling.Server.Models;
using Veiling.Server;
using Microsoft.AspNetCore.Authorization;

namespace Veiling.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BedrijvenController : ControllerBase
    {
        private readonly IAppDbContext _context;

        public BedrijvenController(IAppDbContext context)
        {
            _context = context;
        }

        // GET: api/bedrijven
        [Authorize(Roles = 
        nameof(Role.Administrator) + ", " + 
        nameof(Role.Veilingmeester)
        )]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bedrijf>>> GetBedrijven()
        {
            return await _context.Bedrijven
                .Include(b => b.Gebruikers)  
                .ToListAsync();
        }

//TODO: Bv1 en Vk2 moeten alleen hun eigen bedrijf kunnen opvragen 
        // GET: api/bedrijven/5
        [Authorize(Roles = nameof(Role.Bedrijfsvertegenwoordiger) + ", " + nameof(Role.Veilingmeester) + ", " + nameof(Role.Leverancier) + ", " + nameof(Role.Administrator))]
        [HttpGet("{id}")]
        public async Task<ActionResult<Bedrijf>> GetBedrijf(int id)
        {
            var bedrijf = await _context.Bedrijven
                .Include(b => b.Gebruikers)  
                .FirstOrDefaultAsync(b => b.Bedrijfscode == id);

            if (bedrijf == null)
            {
                return NotFound();
            }

            return bedrijf;
        }

        // POST: api/bedrijven
        [Authorize(Roles = nameof(Role.Administrator))]
        [HttpPost]
        public async Task<ActionResult<Bedrijf>> CreateBedrijf(Bedrijf bedrijf)
        {
            _context.Bedrijven.Add(bedrijf);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBedrijf), new { id = bedrijf.Bedrijfscode }, bedrijf);
        }

        // PUT: api/bedrijven/5
        [Authorize(Roles = nameof(Role.BedrijfManager) + ", " + nameof(Role.Administrator))]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBedrijf(int id, Bedrijf bedrijf)
        {
            if (id != bedrijf.Bedrijfscode)
            {
                return BadRequest();
            }

            _context.Entry(bedrijf).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BedrijfExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/bedrijven/5
        [Authorize(Roles = nameof(Role.Administrator) + ", " + nameof(Role.BedrijfManager))]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBedrijf(int id)
        {
            var bedrijf = await _context.Bedrijven.FindAsync(id);
            if (bedrijf == null)
            {
                return NotFound();
            }

            _context.Bedrijven.Remove(bedrijf);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BedrijfExists(int id)
        {
            return _context.Bedrijven.Any(b => b.Bedrijfscode == id);
        }
    }
}
