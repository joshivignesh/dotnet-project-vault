using Microsoft.AspNetCore.SignalR;

namespace SignalRDashboard.Api.Hubs;

/// <summary>
/// SignalR hub that streams real-time system metrics to connected dashboard clients.
/// Clients subscribe to the "ReceiveMetrics" event to receive updates.
/// </summary>
public class DashboardHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await Clients.Caller.SendAsync("Connected", new
        {
            connectionId = Context.ConnectionId,
            serverTime = DateTime.UtcNow
        });
        await base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception) =>
        base.OnDisconnectedAsync(exception);
}
