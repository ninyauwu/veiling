// Veiling.Server/Controllers/LocatiesController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Veiling.Server.Models;

namespace Veiling.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocatiesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LocatiesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Locatie>>> GetLocaties()
        {
            return await _context.Locaties.ToListAsync();
        }

        [HttpGet("actief")]
        public async Task<ActionResult<IEnumerable<Locatie>>> GetActieveLocaties()
        {
            return await _context.Locaties
                .Where(l => l.Actief)
                .ToListAsync();
        }

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
    }
}