using IPBot.Helpers;

namespace IPBot.Commands;

public class ServerCommands : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("mc", "get the status of the minecraft server")]
    public async Task GetMinecraftServerStatusAsync()
    {
        var serverInfo =
            await ServerInfoHelper.GetServerInfoAsync(Constants.MinecraftServerCode, Constants.MinecraftServerPort);
        
        if (serverInfo != null)
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

    [SlashCommand("ark", "get the status of the ark server")]
    public async Task GetArkServerStatusAsync()
    {
        await DeferAsync();

        var arkServers = await ServerInfoHelper.LoadArkServerDataAsync();
        var serverStatus = new StringBuilder();

        foreach (var (port, map) in arkServers)
        {
            var serverInfo = await ServerInfoHelper.GetServerInfoAsync(Constants.SteamServerCode, port);

            if (serverInfo != null)
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
                if (string.IsNullOrWhiteSpace(map))
                {
                    serverStatus.AppendLine($"{Constants.ServerOfflineString} | Port: {port}");
                }
                else
                {
                    serverStatus.AppendLine(
                        $"Map: {map} - {Constants.ServerOfflineString} | Port: {port}");
                }
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

        var serverStatus = serverInfo == null ? Constants.ServerOfflineString : ServerInfoHelper.PlayerCountStatus(serverInfo.PlayerNames);

        await RespondAsync(serverStatus);
    }
}
