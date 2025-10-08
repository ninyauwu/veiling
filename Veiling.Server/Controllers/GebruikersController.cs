using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Veiling.Server.Data;
using Veiling.Server.Models;

namespace Veiling.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GebruikersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GebruikersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/gebruikers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Gebruiker>>> GetGebruikers()
        {
            return await _context.Gebruikers
                .Include(g => g.Boden)
                .Include(g => g.Kavels)
                .ToListAsync();
        }

        // GET: api/gebruikers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Gebruiker>> GetGebruiker(int id)
        {
            var gebruiker = await _context.Gebruikers
                .Include(g => g.Boden)
                .Include(g => g.Kavels)
                .FirstOrDefaultAsync(g => g.GebruikerId == id);

            if (gebruiker == null)
            {
                return NotFound();
            }

            return gebruiker;
        }

        // POST: api/gebruikers
        [HttpPost]
        public async Task<ActionResult<Gebruiker>> CreateGebruiker(Gebruiker gebruiker)
        {
            _context.Gebruikers.Add(gebruiker);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGebruiker), new { id = gebruiker.GebruikerId }, gebruiker);
        }

        // PUT: api/gebruikers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGebruiker(int id, Gebruiker gebruiker)
        {
            if (id != gebruiker.GebruikerId)
            {
                return BadRequest();
            }

            _context.Entry(gebruiker).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GebruikerExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/gebruikers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGebruiker(int id)
        {
            var gebruiker = await _context.Gebruikers.FindAsync(id);
            if (gebruiker == null)
            {
                return NotFound();
            }

            _context.Gebruikers.Remove(gebruiker);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GebruikerExists(int id)
        {
            return _context.Gebruikers.Any(g => g.GebruikerId == id);
        }
    }
}