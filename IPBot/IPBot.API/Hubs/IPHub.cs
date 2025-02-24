using Microsoft.AspNetCore.SignalR;

namespace IPBot.API.Hubs;

public class IPHub : Hub
{
    public Task SendIP(string ip)
    {
        return Clients.All.SendAsync("UpdateIP", ip);
    }
}