using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Veiling.Server.Models;

namespace Veiling.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AankoopController : ControllerBase {
    private readonly AppDbContext _context;
    private readonly CompleteBidService _completeBidService;

    public AankoopController(AppDbContext context, CompleteBidService completeBidService)
    {
        _context = context;
        _completeBidService = completeBidService;
    }

    [HttpPost]
    public async Task<ActionResult<Aankoop>> KoopContainer(AankoopContainers aankoop) 
    {
        // Valideer dat hoeveelheid positief is
        if (aankoop.Hoeveelheid <= 0) {
            return BadRequest("Hoeveelheid moet groter zijn dan 0");
        }
        
        var kavel = await _context.Kavels.FirstOrDefaultAsync(k => k.Id == aankoop.KavelId);
        if (kavel == null) {
            return NotFound("No kavel with id " + aankoop.KavelId);
        }
        
        var gebruiker = await _context.Gebruikers.FirstOrDefaultAsync(g => g.Id == aankoop.GebruikerId);
        if (gebruiker == null) {
            return NotFound("No gebruiker with id " + aankoop.GebruikerId);
        }
        
        // Boden zijn per definitie al geaccepteerd, dus verdere validatie is niet nodig
        var boden = await _context.Boden
            .Where(b => b.GebruikerId == aankoop.GebruikerId && b.KavelVeiling.KavelId == aankoop.KavelId)
            .ToListAsync();
        
        if (boden.Count < 1) {
            return BadRequest($"User {aankoop.GebruikerId} has not placed any bids on this item.");
        }
        
        // Voorkom dat gebruiker meer koopt dan beschikbaar is
        var aankoopSom = _context.Aankopen
            .Where(a => a.Bod.KavelVeiling.KavelId == aankoop.KavelId)
            .Sum(a => a.Hoeveelheid);
        
        var remaining = kavel.HoeveelheidContainers - aankoopSom;
        
        if (aankoopSom + aankoop.Hoeveelheid > kavel.HoeveelheidContainers) {
            return BadRequest("Kan niet meer containers kopen dan beschikbaar (" + remaining + ")");
        }

        Console.WriteLine("Hahaa" + remaining);
        if ((remaining - aankoop.Hoeveelheid) < 1) {
            kavel.SoldOut = true;
        }
        
        var bod = boden.OrderByDescending(b => b.Datumtijd).First();
        
        var dbAankoop = new Aankoop() {
            Bod = bod,
            BodId = bod.Id,
            Hoeveelheid = aankoop.Hoeveelheid
        };
        
        await _context.Aankopen.AddAsync(dbAankoop);
        
        // ALS ER NOG CONTAINERS OVER ZIJN, PAUZEER DE VEILING IN DE BACKEND
        if ((remaining - aankoop.Hoeveelheid) > 0)
        {
            // Haal de actieve KavelVeiling op
            var now = DateTime.Now;
            var kavelVeiling = await _context.KavelVeilingen
                .Where(kv => kv.KavelId == aankoop.KavelId)
                .Where(kv => kv.Start < now && kv.Start.AddMilliseconds(kv.DurationMs) > now)
                .FirstOrDefaultAsync();
            
            if (kavelVeiling != null)
            {
                // Verschuif de start tijd met 5 seconden in de toekomst
                // Dit zorgt ervoor dat de veiling effectief 5 seconden pauzeert
                kavelVeiling.Start = kavelVeiling.Start.AddSeconds(5);
                _context.KavelVeilingen.Update(kavelVeiling);
                
                Console.WriteLine($"Veiling gepauzeerd: nieuwe start tijd = {kavelVeiling.Start}");
            }
        }
        
        await _context.SaveChangesAsync();

        await _completeBidService.NotifyPurchase(aankoop.KavelId, aankoop.Hoeveelheid, remaining - aankoop.Hoeveelheid);
        
        return dbAankoop;
    }

    public class AankoopContainers {
        public int KavelId { get; set; }
        public string GebruikerId { get; set; }
        public int Hoeveelheid { get; set; }
    }
}
