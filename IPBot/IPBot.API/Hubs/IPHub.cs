using Microsoft.AspNetCore.SignalR;

namespace IPBot.API.Hubs;

internal class IPHub : Hub
{
    public Task SendIP(string ip)
    {
        return Clients.All.SendAsync("UpdateIP", ip);
    }
}