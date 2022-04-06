using System.Diagnostics;
using IPBot.Configs;
using IPBot.Models;

namespace IPBot.Helpers;

internal static class ServerInfoHelper
{
    private static readonly string _arkServerDataFile = Path.Combine(Constants.ConfigDirectory, "ark_server_data.json");
    private static readonly string _serverStatusScriptPath = Path.Combine(Constants.ScriptsDirectory, @"get_server_status.py");

    public static async Task<ServerInfo> GetServerInfoAsync(string gameCode, int port)
    {
        var serverInfo = await GetServerInfoJsonAsync(gameCode, port);
        return ParseServerInfoJson(serverInfo);
    }

    public static string PlayerCountStatus(IEnumerable<string> players)
    {
        var playersList = players.ToList();
        var playerName = playersList.Count > 0
            ? playersList.OrderBy(_ => Guid.NewGuid()).Take(1).First()
            : string.Empty;

        return playersList.Count switch
        {
            0 => $"{Constants.SeverOnlineString} No one is currently playing :)",
            1 => $"{Constants.SeverOnlineString} {playerName} is the only one playing :)",
            2 => $"{Constants.SeverOnlineString} {playerName} and one other are playing :)",
            _ => $"{Constants.SeverOnlineString} {playerName} and {playersList.Count - 1} others are playing :)"
        };
    }
    
    public static string PlayerCountStatus(int playerCount)
    {
        return playerCount switch
        {
            0 => $"{Constants.SeverOnlineString} No one is currently playing :)",
            1 => $"{Constants.SeverOnlineString} One person is playing :)",
            _ => $"{Constants.SeverOnlineString} {playerCount} people are playing :)",
        };
    }
    public static async Task<Dictionary<ushort, string>> LoadArkServerDataAsync()
    {
        if (!File.Exists(_arkServerDataFile))
        {
            await CreateArkDataFileAsync();
        }

        using var file = new StreamReader(_arkServerDataFile);
        return JsonConvert.DeserializeObject<Dictionary<ushort, string>>(await file.ReadToEndAsync());
    }

    public static async Task SaveArkServerDataAsync(Dictionary<ushort, string> servers)
    {
        await using var file = new StreamWriter(_arkServerDataFile);
        await file.WriteAsync(JsonConvert.SerializeObject(servers));
    }

    private static async Task CreateArkDataFileAsync()
    {
        var ports = Resources.ServerPorts.Split(Environment.NewLine).ToDictionary(ushort.Parse, _ => string.Empty);
        await SaveArkServerDataAsync(ports);
    }

    private static async Task<string> GetServerInfoJsonAsync(string gameCode, int portNumber)
    {
        using var process = Process.Start(new ProcessStartInfo
        {
            FileName = "python",
            Arguments = $"{_serverStatusScriptPath} {gameCode} {portNumber}",
            UseShellExecute = false,
            RedirectStandardOutput = true,
        });

        if (process == null)
        {
            return string.Empty;
        }
        
        var result = await process.StandardOutput.ReadToEndAsync();
        return result;
    }

    private static ServerInfo ParseServerInfoJson(string serverInfo)
    {
        return string.IsNullOrWhiteSpace(serverInfo) ? null : JsonConvert.DeserializeObject<ServerInfo>(serverInfo);
    }
}
