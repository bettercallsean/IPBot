using IPBot.Helpers;
using IPBot.Infrastructure.Interfaces;
using IPBot.Infrastructure.Models;

namespace IPBot.Commands;

public class ServerCommands : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IGameServerService _gameServerService;

    public ServerCommands(IGameServerService gameServerService)
    {
        _gameServerService = gameServerService;
    }

    [SlashCommand("mc", "get the status of the minecraft server")]
    public async Task GetMinecraftServerStatusAsync()
    {
        await DeferAsync();

        var serverInfo =
            await _gameServerService.GetMinecraftServerStatusAsync(BotConstants.MinecraftServerPort);

        await PostServerStatusAsync(serverInfo);
    }

    [SlashCommand("ark", "get the status of the ark server")]
    public async Task GetArkServerStatusAsync()
    {
        await DeferAsync();

        var arkServers = await ServerInfoHelper.LoadArkServerDataAsync();
        var serverStatus = new StringBuilder();

        foreach (var (port, map) in arkServers)
        {
            var serverInfo = await _gameServerService.GetSteamServerStatusAsync(port);

            if (serverInfo.Online)
            {
                var playerCountStatus = ServerInfoHelper.PlayerCountStatus(serverInfo.PlayerNames);

                serverStatus.AppendLine(
                    $"Map: {serverInfo.Map} - {playerCountStatus} | Port: {port}");

                if (!string.IsNullOrWhiteSpace(serverInfo.Map))
                {
                    arkServers[port] = serverInfo.Map;
                }
            }
            else
            {
                serverStatus.AppendLine(string.IsNullOrWhiteSpace(map)
                    ? $"{BotConstants.ServerOfflineString} | Port: {port}"
                    : $"Map: {map} - {BotConstants.ServerOfflineString} | Port: {port}");
            }
        }

        serverStatus.AppendLine("\nBloody hell, that's a lot of servers 🦖");
        var serverStatusMessage = serverStatus.ToString();

        await FollowupAsync(serverStatusMessage);
        await ServerInfoHelper.SaveArkServerDataAsync(arkServers);
    }

    [SlashCommand("zomboid", "get the status of the zomboid server")]
    public async Task GetProjectZomboidServerStatusAsync()
    {
        await DeferAsync();

        var serverInfo = await _gameServerService.GetSteamServerStatusAsync(BotConstants.ZomboidServerPort);

        await PostServerStatusAsync(serverInfo);
    }

    private async Task PostServerStatusAsync(ServerInfo serverInfo)
    {
        if (serverInfo is not null)
        {
            if (serverInfo.Online)
            {
                var serverStatus = serverInfo.PlayerNames.Count == 0
                    ? ServerInfoHelper.PlayerCountStatus(serverInfo.PlayerCount)
                    : ServerInfoHelper.PlayerCountStatus(serverInfo.PlayerNames);

                await RespondAsync(serverStatus);
            }
            else
            {
                await RespondAsync(BotConstants.ServerOfflineString);
            }
        }
        else
        {
            await RespondAsync(BotConstants.ServerOfflineString);
        }
    }
}
