using Microsoft.AspNetCore.SignalR;

public class CompleteBidService
{
    private readonly IHubContext<VeilingHub> _hub;

    public CompleteBidService(IHubContext<VeilingHub> hub)
    {
        _hub = hub;
    }

    public async Task NotifyClients(int kavelId, DateTime time)
    {
        Console.WriteLine("Clients notified");
        await _hub.Clients.All.SendAsync("BidPlaced", kavelId);
    }
}
