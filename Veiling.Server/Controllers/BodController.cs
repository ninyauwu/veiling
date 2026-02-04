using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Veiling.Server.Models;

namespace Veiling.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BodController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly CompleteBidService _completeBidService;

    public BodController(AppDbContext context, CompleteBidService completeBidService)
    {
        _context = context;
        _completeBidService = completeBidService;
    }

    // POST: api/boden
    [HttpPost]
    public async Task<ActionResult<bool>> PlaatsBod(GeplaatstBod bod)
    {
        var kavel = await _context.Kavels
            .FirstOrDefaultAsync(k => k.Id == bod.KavelId);
        if (kavel == null) {
            return BadRequest("No Kavel with that ID exists");
        }

        var gebruiker = await _context.Gebruikers
            .FirstOrDefaultAsync(g => g.Id == bod.GebruikerId);
        if (gebruiker == null) {
            return NotFound("No gebruiker with id " + bod.GebruikerId);
        }

        var now = DateTime.Now;
        var kavelLijst = (await _context.KavelVeilingen
            .Include(kv => kv.Kavel)
            .Where(kv => kv.KavelId == bod.KavelId && kv.Id == bod.KavelVeilingId)
            .ToListAsync()).OrderBy(kv => Math.Abs((kv.Start.AddMilliseconds(kv.DurationMs / 2) - now).TotalMilliseconds));
        
        var kavelVerkoop = kavelLijst.FirstOrDefault(kv => kv.Start < now
                    && kv.Start.AddMilliseconds(kv.DurationMs) > now);
        if (kavelVerkoop == null) {
            var nearest = kavelLijst.FirstOrDefault();
            var response = nearest == null ? "No kavels are currently part of this auction." :
                "Kavel is not currently being auctioned at " + now + ". " +
                "Nearest kavel is being auctioned at " + nearest?.Start;
            return BadRequest(response);
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
            GebruikerId = bod.GebruikerId,
            Gebruiker = gebruiker,
            KavelVeilingId = kavelVerkoop.Id,
            KavelVeiling = kavelVerkoop,
        };

        bool firstBid = !_context.Boden.Any(b => b.Datumtijd < databaseBod.Datumtijd
                && b.KavelVeilingId == kavelVerkoop.Id);

        var aankoopSom = _context.Aankopen
            .Where(a => a.Bod.KavelVeiling.KavelId == bod.KavelId)
            .Sum(a => a.Hoeveelheid);
        var remaining = kavel.HoeveelheidContainers - aankoopSom;

        await _context.Boden.AddAsync(databaseBod);
        await _completeBidService.NotifyBid(bod.KavelId, databaseBod.Datumtijd);
        _context.Entry(kavel).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return Ok(new BodResponse(firstBid, price, databaseBod.Datumtijd, remaining));
    }

    class BodResponse {
        public bool Accepted { get; set; }
        public float AcceptedPrice { get; set; }
        public DateTime ReceivedAt { get; set; }
        public int RemainingContainers { get; set; }
        public BodResponse(bool accepted, float acceptedPrice, DateTime receivedAt, int remainingContainers) {
            Accepted = accepted;
            AcceptedPrice = acceptedPrice;
            ReceivedAt = receivedAt;
            RemainingContainers = remainingContainers;
        }
    }
}
