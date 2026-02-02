using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Veiling.Server;

public class CompleteBidService
{
    private readonly IHubContext<VeilingHub> _hub;
    private readonly IServiceProvider _serviceProvider;

    public CompleteBidService(IHubContext<VeilingHub> hub, IServiceProvider serviceProvider)
    {
        _hub = hub;
        _serviceProvider = serviceProvider;
    }

    public async Task NotifyBid(int kavelId, DateTime time)
    {
        Console.WriteLine("Clients notified");
        await _hub.Clients.All.SendAsync("BidPlaced", kavelId);
    }

    public async Task NotifyPurchase(int kavelId, int hoeveelheidGekocht, int hoeveelheidOver) 
    {
        await _hub.Clients.All.SendAsync("ContainersPurchased", kavelId, hoeveelheidOver);
        
        // Als er nog containers over zijn, herstart de veiling na 5 seconden
        if (hoeveelheidOver > 0)
        {
            _ = Task.Run(async () =>
            {
                await Task.Delay(5000); // 5 seconden wachten
                await RestartAuction(kavelId);
            });
        }
    }

    private async Task RestartAuction(int kavelId)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        // Haal de huidige actieve KavelVeiling op
        var now = DateTime.Now;
        var kavelVeiling = await context.KavelVeilingen
            .Include(kv => kv.Kavel)
            .Where(kv => kv.KavelId == kavelId)
            .Where(kv => kv.Start < now && kv.Start.AddMilliseconds(kv.DurationMs) > now)
            .FirstOrDefaultAsync();
        
        if (kavelVeiling == null)
        {
            Console.WriteLine($"Geen actieve veiling gevonden voor kavel {kavelId}");
            return;
        }
        
        // Bereken hoeveel tijd er nog over is
        var elapsed = (now - kavelVeiling.Start).TotalMilliseconds;
        var remainingMs = (int)(kavelVeiling.DurationMs - elapsed);
        
        if (remainingMs <= 0)
        {
            Console.WriteLine($"Geen resterende tijd voor kavel {kavelId}");
            return;
        }
        
        // Bereken de huidige prijs
        var priceFactor = elapsed / kavelVeiling.DurationMs;
        var priceRange = kavelVeiling.Kavel.MaximumPrijs - kavelVeiling.Kavel.MinimumPrijs;
        var currentPrice = (float)(priceRange * (1.0 - priceFactor) + kavelVeiling.Kavel.MinimumPrijs);
        
        // Start de veiling opnieuw met resterende tijd en huidige prijs
        var restartTime = DateTime.Now.AddSeconds(1); // Start over 1 seconde
        
        Console.WriteLine($"Herstart veiling voor kavel {kavelId}: startprijs={currentPrice:F2}, minimumprijs={kavelVeiling.Kavel.MinimumPrijs}, duur={remainingMs}ms");
        
        await _hub.Clients.All.SendAsync(
            "VeilingStart", 
            currentPrice,
            kavelVeiling.Kavel.MinimumPrijs, 
            remainingMs, 
            restartTime, 
            kavelVeiling.Id
        );
    }
}