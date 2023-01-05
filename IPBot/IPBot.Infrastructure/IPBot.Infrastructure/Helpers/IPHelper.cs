namespace IPBot.Infrastructure.Helpers;

public static class IPHelper
{
    private static readonly string LatestIPFilePath = Path.Combine(AppContext.BaseDirectory, @"../latest_ip.txt");
    private static readonly string IPChangedFilePath = Path.Combine(AppContext.BaseDirectory, @"../ip_changed");
    private static readonly string IPMiddlePointFilePath = Path.Combine(AppContext.BaseDirectory, @"../ip_api_middlepoint.txt");
    private static string _ip = string.Empty;
    private static string _ipApiMiddlePointUrl = string.Empty;

    public static async Task<string> GetLocalIPAsync()
    {
        string ip;
        if (File.Exists(IPChangedFilePath))
        {
            ip = await File.ReadAllTextAsync(LatestIPFilePath);
            File.Delete(IPChangedFilePath);
        }
        else if (!string.IsNullOrWhiteSpace(_ip)) return _ip;
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

        _ip = ip.TrimEnd();

        return _ip;
    }

    public static async Task<string> GetSeverIPAsync()
    {
        if (string.IsNullOrWhiteSpace(_ipApiMiddlePointUrl))
            _ipApiMiddlePointUrl = File.ReadAllText(IPMiddlePointFilePath);

        using var httpClient = new HttpClient();
        var ip = await httpClient.GetStringAsync($"{_ipApiMiddlePointUrl}/getIP");

        return ip.Trim();
    }
}