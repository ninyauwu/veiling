using Microsoft.AspNetCore.SignalR;

public sealed class VeilingHub : Hub
{
    public async Task SendVeilingStart(float startingPrice, float minimumPrice, int durationMs, DateTime time) {
        await Clients.All.SendAsync("VeilingStart", startingPrice, minimumPrice, durationMs, time);
    }
}
