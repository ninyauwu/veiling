using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Veiling.Server.Data;
using Veiling.Server.Models;

namespace Veiling.Server.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class GebruikersController : ControllerBase
    {
        private readonly IAppDbContext _context;

        public GebruikersController(IAppDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(
            GebruikerRegistratie dto,
            [FromServices] UserManager<Gebruiker> userManager)
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
            return Ok(new { Message = "User Created Succesfully" });
        }

        [Authorize(Roles = 
        nameof(Role.Administrator)
        )]
        [HttpPut("assign-role")]
        public async Task<IActionResult> AssignRole(
            string userId,
            GebruikerRegistratie dto,
            [FromServices] UserManager<Gebruiker> userManager)
        {
            if (!GebruikerExists(userId.ToString())) return NotFound("User not found");
            
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user == null) return NotFound("User not found");

            // Assign role
            var roleResult = await userManager.AddToRoleAsync(user, dto.Role);
            if (!roleResult.Succeeded) return BadRequest(roleResult.Errors);

            return Ok(new { message = $"Role '{dto.Role}' assigned to user {userId}" });
        }

        // GET: api/gebruikers
        [Authorize(Roles = 
        nameof(Role.Administrator)
        )]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Gebruiker>>> GetGebruikers()
        {
            return await _context.Gebruikers
                .Include(g => g.Bedrijf)
                .Include(g => g.Boden)
                .ToListAsync();
        }

        // GET: api/gebruikers/5
        [Authorize(Roles = 
        nameof(Role.Administrator) + ", " + 
        nameof(Role.Veilingmeester) + ", " + 
        nameof(Role.BedrijfManager) + ", " + 
        nameof(Role.Bedrijfsvertegenwoordiger) + ", " + 
        nameof(Role.Leverancier) + ", " +
        nameof(Role.Gebruiker)
        )]
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
//TODO: Bedrijfmanager moet alleen zijn eigen bedrijf kunnen opvragen
        // GET: api/gebruikers/bedrijf/5
        [Authorize(Roles = 
        nameof(Role.Administrator) + ", " + 
        nameof(Role.Veilingmeester) + ", " + 
        nameof(Role.BedrijfManager)
        )]
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
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<Gebruiker>> CreateGebruiker(Gebruiker gebruiker)
        {
            _context.Gebruikers.Add(gebruiker);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGebruiker), new { id = gebruiker.Id }, gebruiker);
        }
//TODO: iedereen behalve Admin moet alleen zijn eigen gebruiker kunnen aanpassen
        // PUT: api/gebruikers/5
        [Authorize(Roles = 
        nameof(Role.Administrator) + ", " + 
        nameof(Role.Veilingmeester) + ", " + 
        nameof(Role.BedrijfManager) + ", " + 
        nameof(Role.Bedrijfsvertegenwoordiger) + ", " + 
        nameof(Role.Leverancier) + ", " +
        nameof(Role.Gebruiker)
        )]
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
        
//TODO: iedereen behalve Admin moet alleen zijn eigen gebruiker kunnen aanpassen
        // DELETE: api/gebruikers/5
        [Authorize(Roles = 
        nameof(Role.Administrator) + ", " + 
        nameof(Role.Veilingmeester) + ", " + 
        nameof(Role.BedrijfManager) + ", " + 
        nameof(Role.Bedrijfsvertegenwoordiger) + ", " + 
        nameof(Role.Leverancier) + ", " +
        nameof(Role.Gebruiker)
        )]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGebruiker(string id) 
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
