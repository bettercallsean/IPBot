using System.Net;
using IPBot.API.Clients.Interfaces;
using IPBot.API.Domain.Interfaces;
using IPBot.API.Hubs;
using IPBot.Common.Constants;
using IPBot.Common.Services;
using Microsoft.AspNetCore.SignalR;

namespace IPBot.API.Services;

internal class IPService(IDomainRepository domainRepository, IHubContext<IPHub> hubContext, IIPClient ipClient) : IIPService
{
    private readonly IDomainRepository _domainRepository = domainRepository;
    private readonly IHubContext<IPHub> _hubContext = hubContext;
    private readonly IIPClient _ipClient = ipClient;
    private static string _serverIP = string.Empty;

    public async Task<string> GetCurrentServerDomainAsync()
    {
        var domain = await _domainRepository.GetWhereAsync(x => x.Description == "Server Domain");
        return domain.URL;
    }

    public async Task<string> GetLocalIPAsync()
    {
        var ip = await _ipClient.GetLocalIPAsync();
        return ip.TrimEnd();
    }

    public async Task<string> GetServerIPAsync()
    {
        var serverDomain = new Uri($"https://{await GetCurrentServerDomainAsync()}");
        var ips = await Dns.GetHostAddressesAsync(serverDomain.Host);

        _serverIP = ips[0].ToString();

        return _serverIP;
    }

    public async Task<bool> UpdateServerIPAsync(string ip)
    {
        if (ip != null && (!IPAddress.TryParse(ip, out _) || ip.Equals(_serverIP))) return false;

        _serverIP = ip;

        await _hubContext.Clients.All.SendAsync(SignalRHubMethods.UpdateIP, _serverIP);

        return true;
    }
}