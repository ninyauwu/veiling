using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Veiling.Server.Models;

namespace Veiling.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VeilingenController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VeilingenController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/veilingen
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Models.Veiling>>> GetVeilingen()
        {
            return await _context.Veilingen
                .Include(v => v.Veilingmeester)
                    .ThenInclude(vm => vm.Gebruiker)
                .Include(v => v.Kavels)
                .ToListAsync();
        }

        // GET: api/veilingen/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Models.Veiling>> GetVeiling(int id)
        {
            var veiling = await _context.Veilingen
                .Include(v => v.Veilingmeester)
                    .ThenInclude(vm => vm.Gebruiker)
                .Include(v => v.Kavels)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (veiling == null)
            {
                return NotFound();
            }

            return veiling;
        }

        // GET: api/veilingen/actief
        [HttpGet("actief")]
        public async Task<ActionResult<IEnumerable<Models.Veiling>>> GetActieveVeilingen()
        {
            var nu = DateTime.Now;
            return await _context.Veilingen
                .Where(v => v.StartTijd <= nu && v.EndTijd >= nu)
                .Include(v => v.Veilingmeester)
                    .ThenInclude(vm => vm.Gebruiker)
                .Include(v => v.Kavels)
                .ToListAsync();
        }

        // POST: api/veilingen
        [HttpPost]
        public async Task<ActionResult<Models.Veiling>> CreateVeiling(Models.Veiling veiling)
        {
            _context.Veilingen.Add(veiling);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVeiling), new { id = veiling.Id }, veiling);
        }

        // PUT: api/veilingen/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVeiling(int id, Models.Veiling veiling)
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