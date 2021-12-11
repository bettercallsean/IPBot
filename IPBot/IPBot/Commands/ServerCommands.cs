using IPBot.Configs;
using MCServerStatus;
using SteamQueryNet;

namespace IPBot.Commands;

[Group("server")]
public class ServerCommands : ModuleBase
{
    private readonly IMinecraftPinger _minecraftPinger;
    private readonly string _arkServerDataFile = $"{Constants.ConfigDirectory}/ark_server_data.json";

    public ServerCommands(IMinecraftPinger minecraftPinger)
    {
        _minecraftPinger = minecraftPinger;
    }

    [Command("mc")]
    public async Task GetMinecraftServerStatusAsync()
    {
        using (Context.Channel.EnterTypingState())
        {
            var serverStatusTask = _minecraftPinger.PingAsync();

            var responded = Task.WaitAll(new Task[] { serverStatusTask }, 500);

            if (responded)
            {
                var serverStatus = serverStatusTask.Result;

                var playerNames = serverStatus.Players.Sample.Select(x => x.Name);
                await Context.Channel.SendMessageAsync(PlayerCountStatus(playerNames));
            }
            else
            {
                await Context.Channel.SendMessageAsync("The server is currently offline :(");
            }
        }
    }

    [Command("ark")]
    public async Task GetArkServerStatusAsync()
    {
        var arkServers = await LoadArkServerDataAsync();
        var serverStatus = new StringBuilder();

        using (Context.Channel.EnterTypingState())
        {
            foreach (var serverDetails in arkServers)
            {
                using var server = new ServerQuery(Constants.ServerAddress, serverDetails.Key);

                var serverInfoTask = server.GetServerInfoAsync();
                var playerListTask = server.GetPlayersAsync();

                var responded = Task.WaitAll(new Task[] { serverInfoTask, playerListTask }, 500);

                if (responded)
                {
                    var serverInfo = serverInfoTask.Result;
                    var playerList = playerListTask.Result;

                    var playerCountStatus = PlayerCountStatus(playerList.Select(x => x.Name));
                    serverStatus.AppendLine(
                        $"Map: {serverInfo.Map} - {playerCountStatus} | Port: {serverDetails.Key}");

                    if (!string.IsNullOrWhiteSpace(serverInfo.Map))
                        arkServers[serverDetails.Key] = serverInfo.Map;
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(serverDetails.Value))
                    {
                        serverStatus.AppendLine($"The server is currently offline :( | Port: {serverDetails.Key}");
                    }
                    else
                    {
                        serverStatus.AppendLine(
                            $"Map: {serverDetails.Value} - The server is currently offline :( | Port: {serverDetails.Key}");
                    }
                }
            }

            serverStatus.AppendLine("\nBloody hell, that's a lot of servers 🦖");

            await SaveArkServerDataAsync(arkServers);
            await Context.Channel.SendMessageAsync(serverStatus.ToString());
        }
    }

    private string PlayerCountStatus(IEnumerable<string> players)
    {
        var playersList = players.ToList();

        return playersList.Count switch
        {
            0 => "The server is online! No one is currently playing :)",
            1 => $"The server is online! {playersList.OrderBy(x => Guid.NewGuid()).Take(1).First()} is the only one playing :)",
            2 => $"The server is online! {playersList.OrderBy(x => Guid.NewGuid()).Take(1).First()} and one other are playing :)",
            _ => $"The server is online! {playersList.OrderBy(x => Guid.NewGuid()).Take(1).First()} and {playersList.Count - 1} others are playing :)",
        };
    }

    private async Task SaveArkServerDataAsync(Dictionary<ushort, string> servers)
    {
        await using var file = new StreamWriter(_arkServerDataFile);
        await file.WriteAsync(JsonConvert.SerializeObject(servers));
    }

    private async Task<Dictionary<ushort, string>> LoadArkServerDataAsync()
    {
        if (!File.Exists(_arkServerDataFile))
            await CreateArkDataFileAsync();

        using var file = new StreamReader(_arkServerDataFile);
        return JsonConvert.DeserializeObject<Dictionary<ushort, string>>(await file.ReadToEndAsync());
    }

    private async Task CreateArkDataFileAsync()
    {
        var ports = Resources.ServerPorts.Split(Environment.NewLine).ToDictionary(x => ushort.Parse(x), y => string.Empty);
        await SaveArkServerDataAsync(ports);
    }
}

