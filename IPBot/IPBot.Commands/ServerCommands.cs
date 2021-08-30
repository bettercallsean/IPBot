using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using IPBot.Infrastructure;
using MCServerStatus;
using Newtonsoft.Json;
using SteamQueryNet;

namespace IPBot.Commands
{
    [Group("server")]
    public class ServerCommands : ModuleBase
    {
        private readonly IMinecraftPinger _minecraftPinger;

        public ServerCommands(IMinecraftPinger minecraftPinger)
        {
            _minecraftPinger = minecraftPinger;
        }

        [Command("mc")]
        public async Task GetMinecraftServerStatus()
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
                await Context.Channel.SendMessageAsync("The server is currently offline :(");
        }

        [Command("ark")]
        public async Task GetArkServerStatus()
        {
            var arkServers = await LoadArkServerData();
            var serverStatus = new StringBuilder();

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
                    serverStatus.AppendLine(
                        $"Map: {serverDetails.Value} - The server is currently offline :( | Port: {serverDetails.Key}");
            }

            await SaveArkServerData(arkServers);

            serverStatus.AppendLine("\nBloody hell, that's a lot of servers 🦖");
            await Context.Channel.SendMessageAsync(serverStatus.ToString());
        }

        private string PlayerCountStatus(IEnumerable<string> players)
        {
            var playersList = players.ToList();

            if (playersList.Count == 0)
                return "The server is online! No one is currently playing :)";

            var playerName = playersList.OrderBy(x => Guid.NewGuid()).Take(1).First();

            return playersList.Count switch
            {
                1 => $"The server is online! {playerName} is the only one playing :)",
                2 => $"The server is online! {playerName} and one other are playing :)",
                _ => $"The server is online! {playerName} and {playersList.Count - 1} others are playing :)",
            };
        }

        private async Task SaveArkServerData(Dictionary<ushort, string> servers)
        {
            await using var file = new StreamWriter("ark_server_data.json");
            await file.WriteAsync(JsonConvert.SerializeObject(servers));
        }

        private async Task<Dictionary<ushort, string>> LoadArkServerData()
        {
            if (!File.Exists("ark_server_data.json"))
                return new Dictionary<ushort, string> { { 27015, "" }, { 27018, "" }, { 27016, "" } };

            using var file = new StreamReader("ark_server_data.json");
            return JsonConvert.DeserializeObject<Dictionary<ushort, string>>(await file.ReadToEndAsync());
        }
    }
}
