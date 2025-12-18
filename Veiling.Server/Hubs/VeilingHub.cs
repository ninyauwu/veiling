using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Veiling.Server;
using Veiling.Server.Models;

public sealed class VeilingHub : Hub
{
    private AppDbContext _context;

    public VeilingHub(AppDbContext context) {
        _context = context;
    }

    public async Task SendVeilingStart(int kavelId, float startingPrice, float minimumPrice, int durationMs, DateTime time) {
        var kavel = await _context.Kavels.FirstOrDefaultAsync(k => k.Id == kavelId);
        
        if (kavel == null) return;

        var kv = new KavelVeiling() {
            Id = 0,
            KavelId = kavelId,
            Kavel = kavel,
            Start = time,
            DurationMs = durationMs,
        };
        
        kavel.MaximumPrijs = startingPrice;
        kavel.MinimumPrijs = minimumPrice;

        _context.Update(kavel);

        await _context.KavelVeilingen.AddAsync(kv);
        await _context.SaveChangesAsync();
        await Clients.All.SendAsync("VeilingStart", startingPrice, minimumPrice, durationMs, time);
    }
}
