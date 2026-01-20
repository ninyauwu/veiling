using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Veiling.Server.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class KavelInfoController : ControllerBase {
    private readonly IAppDbContext _context;

    public KavelInfoController(IAppDbContext context) {
        _context = context;
    }

    [Authorize(Roles = 
        nameof(Role.Administrator) + ", " + 
        nameof(Role.Veilingmeester) + ", " + 
        nameof(Role.BedrijfManager) + ", " + 
        nameof(Role.Bedrijfsvertegenwoordiger) + ", " + 
        nameof(Role.Leverancier)
    )]
    [HttpGet("{locatieId}")]
    public async Task<ActionResult<IEnumerable<KavelLeverancier>>> GetKavels(int locatieId) {
        var now = DateTime.Now;
        var locatie = await _context.Locaties.FirstOrDefaultAsync(l => l.Id == locatieId);

        if (locatie == null) return NotFound($"Found no location with Id ${locatieId}");

        var veilingen = await _context.Veilingen.Where(v => v.LocatieId == locatieId).ToListAsync();
        var veiling = veilingen.Where(v => v.EndTijd > DateTime.Now).OrderBy(v => v.EndTijd).FirstOrDefault();
        if (veiling == null) return NotFound($"No auctions at ${locatie.Naam} currently active.");

        var kavels = _context.Kavels
            .Where(k => k.VeilingId == locatieId)
            .Include(k => k.Leverancier)
            .Include(k => k.Veiling)
            .Include(k => k.Leverancier.Bedrijf)
            .Where(k => k.VeilingId == veiling.Id)
            .Select(k => new KavelLeverancier(k, k.Leverancier));

        if (kavels == null || kavels.Count() < 1) {
            return NotFound();
        }

        return kavels.ToList();
    }

    [HttpGet("pending")]
    public async Task<ActionResult<IEnumerable<KavelLeverancier>>> GetPendingKavels()
    {
        var kavels = await _context.Kavels
            .Include(k => k.Leverancier)
                .ThenInclude(l => l.Bedrijf)
            .Include(k => k.Veiling)
            .Where(k => k.Approved == null)
            .ToListAsync();

        if (kavels == null || !kavels.Any())
        {
            return NotFound();
        }

        var result = kavels.Select(k => new KavelLeverancier(k, k.Leverancier)).ToList();
        
        return Ok(result);
    }
}
