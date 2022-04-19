using IPBot.Helpers;
using IPBot.Models;

namespace IPBot.Commands;

public class ServerCommands : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("mc", "get the status of the minecraft server")]
    public async Task GetMinecraftServerStatusAsync()
    {
        var serverInfo =
            await ServerInfoHelper.GetServerInfoAsync(Constants.MinecraftServerCode, Constants.MinecraftServerPort);

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
            var serverInfo = await ServerInfoHelper.GetServerInfoAsync(Constants.SteamServerCode, port);

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
                    ? $"{Constants.ServerOfflineString} | Port: {port}"
                    : $"Map: {map} - {Constants.ServerOfflineString} | Port: {port}");
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
        var serverInfo = await ServerInfoHelper.GetServerInfoAsync(Constants.SteamServerCode, Constants.ZomboidServerPort);

        await PostServerStatusAsync(serverInfo);
    }

    private async Task PostServerStatusAsync(ServerInfo serverInfo)
    {
        if (serverInfo is not null)
        {
            if (serverInfo.Online)
            {
                if (serverInfo.PlayerNames == null)
                {
                    await RespondAsync(ServerInfoHelper.PlayerCountStatus(serverInfo.PlayerCount));
                }
                else
                {
                    await RespondAsync(ServerInfoHelper.PlayerCountStatus(serverInfo.PlayerNames));
                }
            }
            else
            {
                await RespondAsync(Constants.ServerOfflineString);
            }
        }
        else
        {
            await RespondAsync(Constants.ServerOfflineString);
        }
    }
}
