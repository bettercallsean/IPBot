using System.Net;
using IPBot.DataServices.Interfaces.DataServices;
using IPBot.Shared.Services;

namespace IPBot.API.Business.Service;

public class IPService : IIPService
{
    private static readonly string LatestIPFilePath = Path.Combine(AppContext.BaseDirectory, @"../latest_ip.txt");
    private static readonly string IPChangedFilePath = Path.Combine(AppContext.BaseDirectory, @"../ip_changed");
    private readonly IDomainDataService _domainDataService;

    private static string _localIp = string.Empty;
    private static string _serverIp = string.Empty;
    
    public IPService(IDomainDataService domainDataService)
    {
        _domainDataService = domainDataService;
    }

    public async Task<string> GetCurrentServerDomainAsync()
    {
        var domain = await _domainDataService.GetByDescriptionAsync("Server Domain");
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
                using var httpClient = new HttpClient();
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
        if (!string.IsNullOrWhiteSpace(_serverIp)) return _serverIp;
        
        var serverDomain = new Uri($"https://{await GetCurrentServerDomainAsync()}");
        var ips = await Dns.GetHostAddressesAsync(serverDomain.Host);

        _serverIp = ips[0].ToString();

        return _serverIp;
    }

    public Task<bool> UpdateServerIPAsync(string ip)
    {
        return Task.Run(() =>
        {
            if (!IPAddress.TryParse(ip, out _) || ip.Equals(_serverIp)) return false;
            
            _serverIp = ip;
            
            return true;
        });
    }
}