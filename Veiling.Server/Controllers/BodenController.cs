using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Veiling.Server.Models;

namespace Veiling.Server.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class BodenController : ControllerBase
    {
        private readonly IAppDbContext _context;

        public BodenController(IAppDbContext context)
        {
            _context = context;
        }

        // GET: api/boden
        [Authorize(Roles = nameof(Role.Veilingmeester) + ", " + 
        nameof(Role.Administrator) + ", " + 
        nameof(Role.BedrijfManager) + ", " + 
        nameof(Role.Bedrijfsvertegenwoordiger) + ", " + 
        nameof(Role.Leverancier)
        )]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bod>>> GetBoden()
        {
            return await _context.Boden
                .Include(b => b.Gebruiker)
                .Include(b => b.KavelVeiling)
                .ToListAsync();
        }
//TODO: Bv1 kan alleen hun eigen boden zien, Vk3 kan al de boden zien die op hun kavel zijn geboden
        // GET: api/boden/5
        [Authorize(Roles = nameof(Role.Veilingmeester) + ", " + 
        nameof(Role.Administrator)
        )]
        [HttpGet("{id}")]
        public async Task<ActionResult<Bod>> GetBod(int id)
        {
            var bod = await _context.Boden
                .Include(b => b.Gebruiker)
                .Include(b => b.KavelVeiling)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bod == null)
            {
                return NotFound();
            }

            return bod;
        }

        // GET: api/boden/kavel/5
        [Authorize(Roles = nameof(Role.Veilingmeester) + ", " + 
        nameof(Role.Administrator) + ", " + 
        nameof(Role.Leverancier)
        )]
        [HttpGet("kavel/{kavelId}")]
        public async Task<ActionResult<IEnumerable<Bod>>> GetBodenByKavel(int kavelId)
        {
            return await _context.Boden
                .Where(b => b.KavelVeiling != null && b.KavelVeiling.Kavel.Id == kavelId)
                .Include(b => b.Gebruiker)
                .OrderByDescending(b => b.Koopprijs)
                .ToListAsync();
        }

        // GET: api/boden/gebruiker/5
        [Authorize(Roles = nameof(Role.Administrator) + ", " + 
        nameof(Role.Veilingmeester)
        )]
        [HttpGet("gebruiker/{gebruikerId}")]
        public async Task<ActionResult<IEnumerable<Bod>>> GetBodenByGebruiker(string gebruikerId)
        {
            return await _context.Boden
                .Where(b => b.GebruikerId == gebruikerId)
                .Include(b => b.KavelVeiling)
                .OrderByDescending(b => b.Datumtijd)
                .ToListAsync();
        }

        // GET: api/boden/hoogste/5
        [Authorize()]
        [HttpGet("hoogste/{kavelId}")]
        public async Task<ActionResult<Bod>> GetHoogsteBod(int kavelId)
        {
            var hoogsteBod = await _context.Boden
                .Where(b => b.KavelVeiling != null && b.KavelVeiling.KavelId == kavelId)
                .Include(b => b.Gebruiker)
                .OrderByDescending(b => b.Koopprijs)
                .FirstOrDefaultAsync();

            if (hoogsteBod == null)
            {
                return NotFound();
            }

            return hoogsteBod;
        }

        // POST: api/boden
        [Authorize(Roles = 
        nameof(Role.Administrator) + ", " + 
        nameof(Role.BedrijfManager) + ", " + 
        nameof(Role.Bedrijfsvertegenwoordiger) + ", " + 
        nameof(Role.Leverancier)
        )]
        [HttpPost]
        public async Task<ActionResult<Bod>> CreateBod(Bod bod)
        {
            bod.Datumtijd = DateTime.Now;
            _context.Boden.Add(bod);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBod), new { id = bod.Id }, bod);
        }

        // PUT: api/boden/5
        [Authorize(Roles = 
        nameof(Role.Administrator) + ", " + 
        nameof(Role.Veilingmeester)
        )]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBod(int id, Bod bod)
        {
            if (id != bod.Id)
            {
                return BadRequest();
            }

            _context.Entry(bod).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BodExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/boden/5
        [Authorize(Roles = 
        nameof(Role.Administrator) + ", " + 
        nameof(Role.Veilingmeester)
        )]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBod(int id)
        {
            var bod = await _context.Boden.FindAsync(id);
            if (bod == null)
            {
                return NotFound();
            }

            _context.Boden.Remove(bod);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BodExists(int id)
        {
            return _context.Boden.Any(b => b.Id == id);
        }
    }
}
