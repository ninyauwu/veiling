using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Veiling.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class KavelInfoController : ControllerBase {
    private readonly AppDbContext _context;

    public KavelInfoController(AppDbContext context) {
        _context = context;
    }

    [HttpGet("{veilingId}")]
    public ActionResult<IEnumerable<KavelLeverancier>> GetKavels(int veilingId) {
        var kavels = _context.Kavels
            .Include(k => k.Leverancier)
            .Include(k => k.Veiling)
            .Include(k => k.Leverancier.Bedrijf)
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
