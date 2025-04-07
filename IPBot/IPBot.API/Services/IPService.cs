using System.Net;
using IPBot.API.Constants;
using IPBot.API.Domain.Interfaces;
using IPBot.API.Hubs;
using IPBot.Common.Constants;
using IPBot.Common.Services;
using Microsoft.AspNetCore.SignalR;

namespace IPBot.API.Services;

public class IPService : IIPService
{
    private readonly IDomainRepository _domainRepository;
    private readonly IHubContext<IPHub> _hubContext;
    private readonly IHttpClientFactory _httpClientFactory;

    public IPService(IDomainRepository domainRepository, IHubContext<IPHub> hubContext, IHttpClientFactory httpClientFactory)
    {
        _domainRepository = domainRepository;
        _hubContext = hubContext;
        _httpClientFactory = httpClientFactory;
    }

    private static readonly string LatestIPFilePath = Path.Combine(AppContext.BaseDirectory, "../latest_ip.txt");
    private static readonly string IPChangedFilePath = Path.Combine(AppContext.BaseDirectory, "../ip_changed");
    private static string _localIp = string.Empty;
    private static string _serverIP = string.Empty;

    public async Task<string> GetCurrentServerDomainAsync()
    {
        var domain = await _domainRepository.GetWhereAsync(x => x.Description == "Server Domain");
        return domain.URL;
    }

    public async Task<string> GetLocalIPAsync()
    {
        string ip;
        if (File.Exists(IPChangedFilePath))
        {
            ip = await File.ReadAllTextAsync(LatestIPFilePath);
            File.Delete(IPChangedFilePath);
        }
        else if (!string.IsNullOrWhiteSpace(_localIp)) return _localIp;
        else
        {
            if (!File.Exists(LatestIPFilePath))
            {
                var httpClient = _httpClientFactory.CreateClient(KeyedHttpClientNames.LocalIPClient);
                ip = await httpClient.GetStringAsync("https://api.ipify.org");
            }
            else
            {
                ip = await File.ReadAllTextAsync(LatestIPFilePath);
            }
        }

        _localIp = ip.TrimEnd();

        return _localIp;
    }

    public async Task<string> GetServerIPAsync()
    {
        if (!string.IsNullOrWhiteSpace(_serverIP)) return _serverIP;

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