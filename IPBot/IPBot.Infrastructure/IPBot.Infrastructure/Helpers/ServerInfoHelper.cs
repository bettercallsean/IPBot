using System.Text.Json;
using IPBot.Infrastructure.Models;

namespace IPBot.Infrastructure.Helpers;

public static class ServerInfoHelper
{
    private const string ServerStatusScriptName = "get_server_status.py";

    public static async Task<ServerInfo> GetServerInfoAsync(string gameCode, int port)
    {
        var serverInfo = await GetServerInfoJsonAsync(gameCode, port);
        return ParseServerInfoJson(serverInfo);
    }

    private static async Task<string> GetServerInfoJsonAsync(string gameCode, int portNumber)
    {
        var serverResults =
            await PythonScriptHelper.RunPythonScriptAsync(ServerStatusScriptName, $"{gameCode} {portNumber}");

        return serverResults;
    }

    private static ServerInfo ParseServerInfoJson(string serverInfo)
    {
        return string.IsNullOrWhiteSpace(serverInfo)
            ? new ServerInfo()
            : JsonSerializer.Deserialize<ServerInfo>(serverInfo);
    }
}