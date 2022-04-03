using System.Diagnostics;
using IPBot.Configs;
using IPBot.Models;

namespace IPBot.Helpers;

internal class ServerInfoHelper
{
    private static readonly string _arkServerDataFile = $"{Constants.ConfigDirectory}/ark_server_data.json";

    public static async Task<ServerInfo> GetServerInfoAsync()
    {
        var serverInfo = await GetServerStatusStringAsync();
        return ParseServerInfoString(serverInfo);
    }

    public static async Task<ServerInfo> GetServerInfoAsync(int port)
    {
        var serverInfo = await GetServerStatusStringAsync(port);
        return ParseServerInfoString(serverInfo);
    }

    public static string PlayerCountStatus(IEnumerable<string> players)
    {
        const string statusString = "The server is online! ";
        var playersList = players.ToList();
        var playerName = playersList.Count > 0
            ? playersList.OrderBy(x => Guid.NewGuid()).Take(1).First()
            : string.Empty;

        return playersList.Count switch
        {
            0 => statusString + "No one is currently playing :)",
            1 => statusString + $"{playerName} is the only one playing :)",
            2 => statusString + $"{playerName} and one other are playing :)",
            _ => statusString + $"{playerName} and {playersList.Count - 1} others are playing :)"
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
        var ports = Resources.ServerPorts.Split(Environment.NewLine).ToDictionary(x => ushort.Parse(x), y => string.Empty);
        await SaveArkServerDataAsync(ports);
    }

    private static async Task<string> GetServerStatusStringAsync(string gameCode, int portNumber)
    {
        var serverStatusScriptPath = Path.Combine(Constants.BaseDirectory, @"../scripts/get_server_status.py");

        if (!File.Exists(serverStatusScriptPath))
        {
            return string.Empty;
        }

        using var process = Process.Start(new ProcessStartInfo
        {
            FileName = "python",
            Arguments = $"{serverStatusScriptPath} {gameCode} {portNumber}",
            UseShellExecute = false,
            RedirectStandardOutput = true,
        });

        using StreamReader reader = process.StandardOutput;

        var result = await reader.ReadToEndAsync();

        return result;
    }

    private static ServerInfo ParseServerInfoString(string serverInfo)
    {
        if (string.IsNullOrWhiteSpace(serverInfo))
        {
            return null;
        }

        return JsonConvert.DeserializeObject<ServerInfo>(serverInfo);
    }
}
