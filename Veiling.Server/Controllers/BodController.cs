using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Veiling.Server.Models;

namespace Veiling.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BodController : ControllerBase
{
    private readonly AppDbContext _context;

    public BodController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/boden
    [HttpPost]
    public async Task<ActionResult<bool>> PlaatsBod(GeplaatstBod bod)
    {
        //var gebruiker = await _context.Gebruikers
        //    .FirstOrDefaultAsync(g => g.Id == bod.GebruikerId);
        //if (gebruiker == null) {
        //    return BadRequest("No Gebruiker with that ID exists.");
        //}
        var kavel = await _context.Kavels
            .FirstOrDefaultAsync(k => k.Id == bod.KavelId);
        if (kavel == null) {
            return BadRequest("No Kavel with that ID exists");
        }
        var kavelVerkoop = await _context.KavelVeilingen
            .Include(kv => kv.Kavel)
            .FirstOrDefaultAsync(kv => kv.KavelId == bod.KavelId);
        if (kavelVerkoop == null
                || kavelVerkoop.Start > DateTime.Now
                || kavelVerkoop.Start.AddMilliseconds(kavelVerkoop.DurationMs) < DateTime.Now) {
            return BadRequest("Kavel is not currently being auctioned.");
        }

        var timespan = DateTime.Now - kavelVerkoop.Start;
        var priceFactor = timespan.TotalMilliseconds / (double)kavelVerkoop.DurationMs;
        var priceRange = kavelVerkoop.Kavel.MaximumPrijs - kavelVerkoop.Kavel.MinimumPrijs;
        var price = (float)(priceRange * (1.0 - priceFactor) + kavelVerkoop.Kavel.MinimumPrijs);

        Bod databaseBod = new Bod {
            Datumtijd = DateTime.Now,
            HoeveelheidContainers = bod.HoeveelheidContainers ?? 0,
            Koopprijs = price,
            Betaald = false,

            GebruikerId = "",
            Gebruiker = null,
            KavelId = bod.KavelId,
            Kavel = kavel,
        };

        bool firstBid = !_context.Boden.Any(b => b.KavelId == bod.KavelId && b.Datumtijd < databaseBod.Datumtijd);

        await _context.Boden.AddAsync(databaseBod);

        return Ok(new BodResponse(firstBid, databaseBod.Datumtijd));
    }

    class BodResponse {
        public bool Accepted { get; set; }
        public DateTime ReceivedAt { get; set; }

        public BodResponse(bool accepted, DateTime receivedAt) {
            Accepted = accepted;
            ReceivedAt = receivedAt;
        }
    }
}
