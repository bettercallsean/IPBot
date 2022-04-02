using System.Diagnostics;
using IPBot.Helpers;

namespace IPBot.Commands;

public class ServerCommands : InteractionModuleBase<SocketInteractionContext>
{
    public ServerCommands()
    {
    }

    [SlashCommand("mc", "get the staus of the minecraft server")]
    public async Task GetMinecraftServerStatusAsync()
    {
        using (Context.Channel.EnterTypingState())
        {
            var serverStatus = await ServerInfoHelper.GetServerInfoAsync();

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

        var timer = new Stopwatch();
        timer.Start();
        foreach (var serverDetails in arkServers)
        {
            var serverInfo = await ServerInfoHelper.GetServerInfoAsync(serverDetails.Key);
            Console.WriteLine($"Server Info - {timer.ElapsedMilliseconds}");
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

        Console.WriteLine(timer.ElapsedMilliseconds);

        await FollowupAsync(serverStatusMessage);
        await ServerInfoHelper.SaveArkServerDataAsync(arkServers);
    }

    [SlashCommand("zomboid", "get the status of the zomboid server")]
    public async Task GetProjectZomboidServerStatusAsync()
    {
        var serverInfo = await ServerInfoHelper.GetServerInfoAsync(Constants.ZomboidServerPort);

        var serverStatus = serverInfo == null ? Constants.ServerOfflineString : ServerInfoHelper.PlayerCountStatus(serverInfo.PlayerNames);

        await RespondAsync(serverStatus);
    }
}
