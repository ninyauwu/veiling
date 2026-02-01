using Microsoft.AspNetCore.SignalR;

public class CompleteBidService
{
    private readonly IHubContext<VeilingHub> _hub;

    public CompleteBidService(IHubContext<VeilingHub> hub)
    {
        _hub = hub;
    }

    public async Task NotifyBid(int kavelId, DateTime time)
    {
        Console.WriteLine("Clients notified");
        await _hub.Clients.All.SendAsync("BidPlaced", kavelId);
    }

    public async Task NotifyPurchase(int kavelId, int hoeveelheidGekocht, int hoeveelheidOver) {
        await _hub.Clients.All.SendAsync("ContainersPurchased", kavelId, hoeveelheidOver);
    }
}
