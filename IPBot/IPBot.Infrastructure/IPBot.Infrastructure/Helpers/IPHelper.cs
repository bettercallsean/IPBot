using System.Net;

namespace IPBot.Infrastructure.Helpers;

public static class IPHelper
{
    private static readonly string LatestIPFilePath = Path.Combine(AppContext.BaseDirectory, @"../latest_ip.txt");
    private static readonly string IPChangedFilePath = Path.Combine(AppContext.BaseDirectory, @"../ip_changed");
    private static readonly string IpMiddleManApiUrl = DotEnvHelper.EnvironmentVariables["IP_MIDDLEMAN_URL"];
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

    public static Task<string> GetSeverIPAsync()
    {
        return Task.FromResult(_serverIp);
    }

    public static bool UpdateServerIP(string ip)
    {
        if (!IPAddress.TryParse(ip, out var _) || ip.Equals(_serverIp))
            return false;

        _serverIp = ip;

        return true;
    }
}