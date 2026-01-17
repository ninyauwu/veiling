using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Veiling.Server.Models;

namespace Veiling.Server.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class LeveranciersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LeveranciersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/leveranciers
        [Authorize(Roles = 
        nameof(Role.Administrator) + ", " + 
        nameof(Role.Veilingmeester)
        )]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Leverancier>>> GetLeveranciers()
        {
            return await _context.Leveranciers
                .Include(l => l.Bedrijf)
                .Include(l => l.Kavels)
                .ToListAsync();
        }

        [Authorize(Roles = 
    nameof(Role.Administrator) + ", " + 
    nameof(Role.Veilingmeester) + ", " + 
    nameof(Role.Leverancier)
)]
[HttpGet("mijn/kavels")]
public async Task<ActionResult<IEnumerable<KavelListDto>>> GetMijnKavels(
    [FromServices] UserManager<Gebruiker> userManager)
{
    var user = await userManager.GetUserAsync(User);
    if (user == null || user.BedrijfId == null)
        return Unauthorized();

    var leverancier = await _context.Leveranciers
        .Include(l => l.Kavels)
        .FirstOrDefaultAsync(l => l.BedrijfId == user.BedrijfId);

    if (leverancier == null)
        return NotFound("Geen leverancier gevonden voor dit bedrijf");

    // 3️⃣ Map de kavels naar DTO
    var kavelsDto = leverancier.Kavels.Select(k => new KavelListDto
    {
        Id = k.Id,
        Title = k.Naam,
        Price = (decimal)k.MinimumPrijs,
        Location = k.LocatieId.ToString() // of k.Veiling.Naam als je naam wilt
    }).ToList();

    return Ok(kavelsDto);
}


//TODO: Vk3 moet alleen zijn eigen leverancier kunnen opvragen
        // GET: api/leveranciers/5
        [Authorize(Roles = 
        nameof(Role.Administrator) + ", " + 
        nameof(Role.Veilingmeester) + ", " + 
        nameof(Role.Leverancier)
        )]
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
        [Authorize(Roles = 
        nameof(Role.Administrator)
        )]
        [HttpPost]
        public async Task<ActionResult<Leverancier>> CreateLeverancier(Leverancier leverancier)
        {
            _context.Leveranciers.Add(leverancier);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLeverancier), new { id = leverancier.Id }, leverancier);
        }

        // PUT: api/leveranciers/5
        [Authorize(Roles = 
        nameof(Role.Administrator)
        )]
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
        [Authorize(Roles = 
        nameof(Role.Administrator)
        )]
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