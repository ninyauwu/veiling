using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Veiling.Server.Models;

namespace Veiling.Server.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class LocatiesController : ControllerBase
    {
        private readonly IAppDbContext _context;

        public LocatiesController(IAppDbContext context)
        {
            _context = context;
        }

        // GET: api/locaties
        [Authorize(Roles = 
        nameof(Role.Administrator) + ", " + 
        nameof(Role.Veilingmeester) + ", " + 
        nameof(Role.BedrijfManager) + ", " + 
        nameof(Role.Bedrijfsvertegenwoordiger) + ", " + 
        nameof(Role.Leverancier)
        )]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Locatie>>> GetLocaties()
        {
            return await _context.Locaties.ToListAsync();
        }

        // GET: api/locaties/actief
        [Authorize(Roles = 
        nameof(Role.Administrator) + ", " + 
        nameof(Role.Veilingmeester) + ", " + 
        nameof(Role.BedrijfManager) + ", " + 
        nameof(Role.Bedrijfsvertegenwoordiger) + ", " + 
        nameof(Role.Leverancier)
        )]
        [HttpGet("actief")]
        public async Task<ActionResult<IEnumerable<Locatie>>> GetActieveLocaties()
        {
            return await _context.Locaties
                .Where(l => l.Actief)
                .ToListAsync();
        }

        // GET: api/locaties/5
        [Authorize(Roles = 
        nameof(Role.Administrator) + ", " + 
        nameof(Role.Veilingmeester) + ", " + 
        nameof(Role.BedrijfManager) + ", " + 
        nameof(Role.Bedrijfsvertegenwoordiger) + ", " + 
        nameof(Role.Leverancier)
        )]
        [HttpGet("{id}")]
        public async Task<ActionResult<Locatie>> GetLocatie(int id)
        {
            var locatie = await _context.Locaties
                .Include(l => l.Veilingen)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (locatie == null)
            {
                return NotFound();
            }

            return locatie;
        }

        // PUT: api/locaties/5
        [Authorize(Roles = 
        nameof(Role.Administrator) + ", " + 
        nameof(Role.Veilingmeester)
        )]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLocatie(int id, Locatie locatie)
        {
            if (id != locatie.Id)
            {
                return BadRequest();
            }

            _context.Entry(locatie).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LocatieExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }
        
        // POST: api/locaties
        [Authorize(Roles = 
            nameof(Role.Administrator) + ", " + 
            nameof(Role.Veilingmeester)
        )]
        [HttpPost]
        public async Task<ActionResult<Locatie>> CreateLocatie(Locatie locatie)
        {
            _context.Locaties.Add(locatie);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLocatie), new { id = locatie.Id }, locatie);
        }

        // DELETE: api/locaties/5
        [Authorize(Roles = 
            nameof(Role.Administrator) + ", " + 
            nameof(Role.Veilingmeester)
        )]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLocatie(int id)
        {
            var locatie = await _context.Locaties.FindAsync(id);
            if (locatie == null)
            {
                return NotFound();
            }

            _context.Locaties.Remove(locatie);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LocatieExists(int id)
        {
            return _context.Locaties.Any(l => l.Id == id);
        }
    }
}
