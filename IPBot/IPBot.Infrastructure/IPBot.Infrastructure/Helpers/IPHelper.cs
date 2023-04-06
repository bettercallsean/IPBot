using System.Net;

namespace IPBot.Infrastructure.Helpers;

public static class IPHelper
{
    private static readonly string LatestIPFilePath = Path.Combine(AppContext.BaseDirectory, @"../latest_ip.txt");
    private static readonly string IPChangedFilePath = Path.Combine(AppContext.BaseDirectory, @"../ip_changed");
    private static string _localIp = string.Empty;
    private static string _serverIp = string.Empty;

    public static async Task<string> GetLocalIPAsync()
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

    public static async Task<string> GetSeverIPAsync()
    {
        if (string.IsNullOrWhiteSpace(_serverIp))
        {
            var serverDomain = new Uri($"https://{DotEnvHelper.EnvironmentVariables["CURRENT_DOMAIN"]}");
            var ips = await Dns.GetHostAddressesAsync(serverDomain.Host);

            _serverIp = ips[0].ToString();
        }

        return _serverIp;
    }

    public static bool UpdateServerIP(string ip)
    {
        if (!IPAddress.TryParse(ip, out var _) || ip.Equals(_serverIp))
            return false;

        _serverIp = ip;

        return true;
    }
}