using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Veiling.Server.Models;

namespace Veiling.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeveranciersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LeveranciersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/leveranciers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Leverancier>>> GetLeveranciers()
        {
            return await _context.Leveranciers
                .Include(l => l.Bedrijf)
                .Include(l => l.Kavels)
                .ToListAsync();
        }

        // GET: api/leveranciers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Leverancier>> GetLeverancier(int id)
        {
            var leverancier = await _context.Leveranciers
                .Include(l => l.Bedrijf)
                .Include(l => l.Kavels)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (leverancier == null)
            {
                return NotFound();
            }

            return leverancier;
        }

        // POST: api/leveranciers
        [HttpPost]
        public async Task<ActionResult<Leverancier>> CreateLeverancier(Leverancier leverancier)
        {
            _context.Leveranciers.Add(leverancier);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLeverancier), new { id = leverancier.Id }, leverancier);
        }

        // PUT: api/leveranciers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLeverancier(int id, Leverancier leverancier)
        {
            if (id != leverancier.Id)
            {
                return BadRequest();
            }

            _context.Entry(leverancier).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LeverancierExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/leveranciers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLeverancier(int id)
        {
            var leverancier = await _context.Leveranciers.FindAsync(id);
            if (leverancier == null)
            {
                return NotFound();
            }

            _context.Leveranciers.Remove(leverancier);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LeverancierExists(int id)
        {
            return _context.Leveranciers.Any(l => l.Id == id);
        }
    }
}