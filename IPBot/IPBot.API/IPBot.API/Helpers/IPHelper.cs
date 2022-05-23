namespace IPBot.API.Helpers;

public static class IPHelper
{
    private static readonly string IPFilePath = Path.Combine(AppContext.BaseDirectory, @"../latest_ip.txt");
    private static readonly string IPChangedFilePath = Path.Combine(AppContext.BaseDirectory, @"../ip_changed");
    private static string _ip = string.Empty;

    public static async Task<string> GetIPFromFileAsync()
    {
        string ip;
        if (File.Exists(IPChangedFilePath))
        {
            ip = await File.ReadAllTextAsync(IPFilePath);
        }
        else if (!string.IsNullOrWhiteSpace(_ip))
        {
            return _ip;
        }
        else
        {
            ip = !File.Exists(IPFilePath)
                ? await PythonScriptHelper.RunPythonScriptAsync("get_ip.py")
                : await File.ReadAllTextAsync(IPFilePath);
        }

        _ip = ip.TrimEnd();

        return _ip;
    }
}