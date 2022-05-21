namespace IPBot.API.Helpers;

public static class IPHelper
{
    private static readonly string IPFilePath = Path.Combine(Constants.BaseDirectory, @"../latest_ip.txt");
    private static string _ip = string.Empty;
    
    public static async Task<string> GetIPFromFileAsync()
    {
        if (!string.IsNullOrWhiteSpace(_ip)) return _ip;
        
        if (!File.Exists(IPFilePath))
        {
            _ip = await PythonScriptHelper.RunPythonScriptAsync("get_ip.py");
        }
        else
        {
            _ip = await File.ReadAllTextAsync(IPFilePath);
        }

        _ip = _ip.TrimEnd();

        return _ip;
    }
}