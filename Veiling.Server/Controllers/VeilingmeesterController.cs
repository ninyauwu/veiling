using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Veiling.Server.Models;

namespace Veiling.Server.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class VeilingmeestersController : ControllerBase
    {
        private readonly IAppDbContext _context;

        public VeilingmeestersController(IAppDbContext context)
        {
            _context = context;
        }

        // GET: api/veilingmeesters
        [HttpGet]
        [Authorize(Roles = 
        nameof(Role.Administrator) + ", " + 
        nameof(Role.Veilingmeester)
        )]
        public async Task<ActionResult<IEnumerable<Veilingmeester>>> GetVeilingmeesters()
        {
            return await _context.Veilingmeesters
                .Include(v => v.Gebruiker)
                .Include(v => v.Veilingen)
                .ToListAsync();
        }

        // GET: api/veilingmeesters/5
        [Authorize(Roles = 
        nameof(Role.Administrator) + ", " + 
        nameof(Role.Veilingmeester)
        )]
        [HttpGet("{id}")]
        public async Task<ActionResult<Veilingmeester>> GetVeilingmeester(int id)
        {
            var veilingmeester = await _context.Veilingmeesters
                .Include(v => v.Gebruiker)
                .Include(v => v.Veilingen)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (veilingmeester == null)
            {
                return NotFound();
            }

            return veilingmeester;
        }

        // POST: api/veilingmeesters
        [Authorize(Roles = 
        nameof(Role.Administrator) + ", " + 
        nameof(Role.Veilingmeester)
        )]
        [HttpPost]
        public async Task<ActionResult<Veilingmeester>> CreateVeilingmeester(Veilingmeester veilingmeester)
        {
            _context.Veilingmeesters.Add(veilingmeester);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVeilingmeester), new { id = veilingmeester.Id }, veilingmeester);
        }

//TODO: Vm4 moet alleen zich zelf kunnen verranderen
        // PUT: api/veilingmeesters/5
        [Authorize(Roles = 
        nameof(Role.Administrator) + ", " + 
        nameof(Role.Veilingmeester)
        )]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVeilingmeester(int id, Veilingmeester veilingmeester)
        {
            if (id != veilingmeester.Id)
            {
                return BadRequest();
            }

            _context.Entry(veilingmeester).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VeilingmeesterExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/veilingmeesters/5
        [Authorize(Roles = 
        nameof(Role.Administrator)
        )]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVeilingmeester(int id)
        {
            var veilingmeester = await _context.Veilingmeesters.FindAsync(id);
            if (veilingmeester == null)
            {
                return NotFound();
            }

            _context.Veilingmeesters.Remove(veilingmeester);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VeilingmeesterExists(int id)
        {
            return _context.Veilingmeesters.Any(v => v.Id == id);
        }
    }
}
