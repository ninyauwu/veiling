using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Veiling.Server;
using Veiling.Server.Models;

[ApiController]
[Route("api/[controller]")]
public class KavelVeilingController : ControllerBase {
    private AppDbContext _context;

    public KavelVeilingController(AppDbContext context) {
        _context = context;
    }

    [Authorize]
    [HttpGet("kavel/{kavelId}")]
    public async Task<ActionResult<KavelVeiling>> GetResult(int kavelId) {
        var kavelVeiling = _context.KavelVeilingen.FirstOrDefault(kv => kv.KavelId == kavelId);

        if (kavelVeiling == null) {
            return NotFound($"No KavelVeiling found for Kavel {kavelId}.");
        }

        return kavelVeiling;
    }
}
