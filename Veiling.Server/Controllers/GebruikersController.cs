using Microsoft.AspNetCore.Identity;
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
        private readonly AppDbContext _context;

        public GebruikersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(GebruikerRegistratie dto, [FromServices] UserManager<Gebruiker> userManager)
        {
            var user = new Gebruiker
            {
                UserName = dto.Email,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Name = dto.FirstName + " " + dto.LastName,
            };

            var result = await userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("User registered successfully");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(GebruikerLogin dto,
            [FromServices] SignInManager<Gebruiker> signInManager)
        {
            var result = await signInManager.PasswordSignInAsync(
                dto.Email,
                dto.Password,
                isPersistent: false,
                lockoutOnFailure: false);

            if (!result.Succeeded)
                return Unauthorized("Invalid login.");

            return Ok("Logged in successfully");
        }

        // GET: api/gebruikers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Gebruiker>>> GetGebruikers()
        {
            return await _context.Gebruikers
                .Include(g => g.Bedrijf)
                .Include(g => g.Boden)
                .ToListAsync();
        }

        // GET: api/gebruikers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Gebruiker>> GetGebruiker(string id)
        {
            var gebruiker = await _context.Gebruikers
                .Include(g => g.Bedrijf)
                .Include(g => g.Boden)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (gebruiker == null)
            {
                return NotFound();
            }

            return gebruiker;
        }

        // GET: api/gebruikers/bedrijf/5
        [HttpGet("bedrijf/{bedrijfId}")]
        public async Task<ActionResult<IEnumerable<Gebruiker>>> GetGebruikersByBedrijf(int bedrijfId)
        {
            return await _context.Gebruikers
                .Where(g => g.BedrijfId == bedrijfId)
                .Include(g => g.Bedrijf)
                .Include(g => g.Boden)
                .ToListAsync();
        }

        // POST: api/gebruikers
        [HttpPost]
        public async Task<ActionResult<Gebruiker>> CreateGebruiker(Gebruiker gebruiker)
        {
            _context.Gebruikers.Add(gebruiker);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGebruiker), new { id = gebruiker.Id }, gebruiker);
        }

        // PUT: api/gebruikers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGebruiker(string id, Gebruiker gebruiker)
        {
            if (id != gebruiker.Id)
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

        private bool GebruikerExists(string id)
        {
            return _context.Gebruikers.Any(g => g.Id == id);
        }
    }
}