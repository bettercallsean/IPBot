﻿using IPBot.Configs;
using MCServerStatus;
using SteamQueryNet;
using SteamQueryNet.Models;

namespace IPBot.Commands;

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

                var playerNames = serverStatus.Players.Online > 0
                    ? serverStatus.Players.Sample.Select(x => x.Name)
                    : new List<string>();
                await Context.Channel.SendMessageAsync(PlayerCountStatus(playerNames));
            }
            else
            {
                await Context.Channel.SendMessageAsync(Constants.ServerOfflineString);
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
                var serverInfo = GetSteamServerInfo(serverDetails.Key);
                var playerList = GetSteamServerPlayerList(serverDetails.Key);

                if (serverInfo != null)
                {
                    var playerCountStatus = PlayerCountStatus(playerList.Select(x => x.Name));

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

            await SaveArkServerDataAsync(arkServers);
            await Context.Channel.SendMessageAsync(serverStatus.ToString());
        }
    }

    [Command("zomboid")]
    public async Task GetProjectZomboidServerStatusAsync()
    {
        var playerList = GetSteamServerPlayerList(Constants.ZomboidServerPort);

        var serverStatus = playerList == null ? Constants.ServerOfflineString : PlayerCountStatus(playerList.Select(x => x.Name));

        await Context.Channel.SendMessageAsync(serverStatus);
    }

    private string PlayerCountStatus(IEnumerable<string> players)
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

    private async Task SaveArkServerDataAsync(Dictionary<ushort, string> servers)
    {
        await using var file = new StreamWriter(_arkServerDataFile);
        await file.WriteAsync(JsonConvert.SerializeObject(servers));
    }

    private async Task<Dictionary<ushort, string>> LoadArkServerDataAsync()
    {
        if (!File.Exists(_arkServerDataFile))
        {
            await CreateArkDataFileAsync();
        }

        using var file = new StreamReader(_arkServerDataFile);
        return JsonConvert.DeserializeObject<Dictionary<ushort, string>>(await file.ReadToEndAsync());
    }

    private async Task CreateArkDataFileAsync()
    {
        var ports = Resources.ServerPorts.Split(Environment.NewLine).ToDictionary(x => ushort.Parse(x), y => string.Empty);
        await SaveArkServerDataAsync(ports);
    }

    private List<Player> GetSteamServerPlayerList(ushort port)
    {
        using var server = new ServerQuery(Constants.ServerAddress, port);

        var playerListTask = server.GetPlayersAsync();

        var responded = Task.WaitAll(new Task[] { playerListTask }, 500);

        if (responded)
        {
            return playerListTask.Result;
        }
        else
        {
            return null;
        }
    }

    private ServerInfo GetSteamServerInfo(ushort port)
    {
        using var server = new ServerQuery(Constants.ServerAddress, port);

        var serverInfoTask = server.GetServerInfoAsync();

        var responded = Task.WaitAll(new Task[] { serverInfoTask }, 500);

        if (responded)
        {
            return serverInfoTask.Result;
        }
        else
        {
            return null;
        }
    }
}