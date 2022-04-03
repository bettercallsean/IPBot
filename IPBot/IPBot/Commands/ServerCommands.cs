using IPBot.Helpers;

namespace IPBot.Commands;

public class ServerCommands : InteractionModuleBase<SocketInteractionContext>
{
    public ServerCommands()
    {
    }

    [SlashCommand("mc", "get the status of the minecraft server")]
    public async Task GetMinecraftServerStatusAsync()
    {
        using (Context.Channel.EnterTypingState())
        {
            var serverStatus = await ServerInfoHelper.GetServerInfoAsync(Constants.MinecraftServerCode, Constants.MinecraftServerPort);

            if (serverStatus != null)
            {
                await RespondAsync(ServerInfoHelper.PlayerCountStatus(serverStatus.PlayerNames));
            }
            else
            {
                await RespondAsync(Constants.ServerOfflineString);
            }
        }
    }

    [SlashCommand("ark", "get the status of the ark server", true, RunMode.Async)]
    public async Task GetArkServerStatusAsync()
    {
        await DeferAsync();

        var arkServers = await ServerInfoHelper.LoadArkServerDataAsync();
        var serverStatus = new StringBuilder();

        foreach (var serverDetails in arkServers)
        {
            var serverInfo = await ServerInfoHelper.GetServerInfoAsync(Constants.SteamServerCode, serverDetails.Key);

            if (serverInfo != null)
            {
                var playerCountStatus = ServerInfoHelper.PlayerCountStatus(serverInfo.PlayerNames);

                serverStatus.AppendLine(
                    $"Map: {serverInfo.Map} - {playerCountStatus} | Port: {serverDetails.Key}");

                if (!string.IsNullOrWhiteSpace(serverInfo.Map))
                {
                    arkServers[serverDetails.Key] = serverInfo.Map;
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(serverDetails.Value))
                {
                    serverStatus.AppendLine($"{Constants.ServerOfflineString} | Port: {serverDetails.Key}");
                }
                else
                {
                    serverStatus.AppendLine(
                        $"Map: {serverDetails.Value} - {Constants.ServerOfflineString} | Port: {serverDetails.Key}");
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
