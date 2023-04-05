using Discord;
using IPBot.Helpers;
using IPBot.Infrastructure.Helpers;
using IPBot.Infrastructure.Interfaces;
using IPBot.Infrastructure.Models;

namespace IPBot.Commands;

public class ServerCommands : InteractionModuleBase<SocketInteractionContext>
{
    private const string GameServerMenu = "gameServerMenu";

    private readonly IGameServerService _gameServerService;

    public ServerCommands(IGameServerService gameServerService)
    {
        _gameServerService = gameServerService;
    }

#if DEBUG
    [SlashCommand("mc_debug", "get the status of the minecraft server")]
#else
    [SlashCommand("mc", "get the status of the minecraft server")]
#endif
    public async Task GetMinecraftServerStatusAsync()
    {
        await DeferAsync();

        var serverInfo =
            await _gameServerService.GetMinecraftServerStatusAsync(BotConstants.MinecraftServerPort);

        var serverStatus = GetServerStatus(serverInfo);

        await FollowupAsync(serverStatus);
    }

#if DEBUG
    [SlashCommand("ark_debug", "get the status of the ark server")]
#else
    [SlashCommand("ark", "get the status of the ark server")]
#endif
    public async Task GetArkServerStatusAsync()
    {
        await DeferAsync();

        var arkServers = await ServerInfoHelper.LoadArkServerDataAsync();
        var serverStatus = new StringBuilder();
        var activeServers = new Dictionary<string, int>();

        foreach (var (port, map) in arkServers)
        {
            var serverInfo = await _gameServerService.GetSteamServerStatusAsync(port);
            var playerCountStatus = GetServerStatus(serverInfo);
            var serverMapHasValue = !string.IsNullOrWhiteSpace(serverInfo.Map);

            serverStatus.AppendLine(serverMapHasValue
                    ? $"Map: {serverInfo.Map} - {playerCountStatus} | Port: {port}"
                    : $"{(string.IsNullOrEmpty(map) ? string.Empty : $"Map: {map} - ")}{playerCountStatus} | Port: {port}");

            if(serverInfo.Online)
                activeServers.Add(serverInfo.Map, port);

            if (!string.IsNullOrWhiteSpace(serverInfo.Map) && !serverInfo.Map.Equals(map))
                arkServers[port] = serverInfo.Map;
        }

        serverStatus.AppendLine($"{Environment.NewLine}Bloody hell, that's a lot of servers 🦖");
        var serverStatusMessage = serverStatus.ToString();

        if (activeServers.Any())
        {
            var component = CreateGameServerMenuComponent(activeServers);
            await FollowupAsync(serverStatusMessage, components: component.Build());
        }
        else
        {
            await FollowupAsync(serverStatusMessage);
        }

        await ServerInfoHelper.SaveArkServerDataAsync(arkServers);
    }

#if DEBUG
    [SlashCommand("zomboid_debug", "get the status of the zomboid server")]
#else
    [SlashCommand("zomboid", "get the status of the zomboid server")]
#endif
    public async Task GetProjectZomboidServerStatusAsync()
    {
        await DeferAsync();

        var serverInfo = await _gameServerService.GetSteamServerStatusAsync(BotConstants.ZomboidServerPort);

        var serverStatus = GetServerStatus(serverInfo);

        if (serverInfo.Online)
        {
            var component = CreateGameServerMenuComponent(serverInfo.Map, BotConstants.ZomboidServerPort);
            await FollowupAsync(serverStatus, components: component.Build());
        }
        else
        {
            await FollowupAsync(serverStatus);
        }
    }

    [ComponentInteraction(GameServerMenu)]
    public async Task GenerateSteamConnectLinkAsync(string[] inputs)
    {
        var serverDomain = ServerDomainHelper.GetCurrentServerDomain();
        await RespondAsync($"Open up Steam after clicking this link and you should see the 'server connect' menu{Environment.NewLine}" +
            $"steam://connect/{serverDomain}:{inputs[0]}", ephemeral: true);
    }

    private static string GetServerStatus(ServerInfo serverInfo)
    {
        if (serverInfo is not null)
        {
            if (serverInfo.Online)
            {
                var serverStatus = serverInfo.PlayerNames.Count == 0
                    ? ServerInfoHelper.PlayerCountStatus(serverInfo.PlayerCount)
                    : ServerInfoHelper.PlayerCountStatus(serverInfo.PlayerNames);

                return serverStatus;
            }
            else
            {
                return BotConstants.ServerOfflineString;
            }
        }
        else
        {
            return BotConstants.ServerOfflineString;
        }
    }

    private static ComponentBuilder CreateGameServerMenuComponent(string mapName, int port)
    {
        var mapsAndPorts = new Dictionary<string, int>
        {
            { mapName, port }
        };

        return CreateGameServerMenuComponent(mapsAndPorts);
    }

    private static ComponentBuilder CreateGameServerMenuComponent(Dictionary<string, int> mapsAndPorts)
    {
        var serverMenu = new SelectMenuBuilder
        {
            CustomId = "gameServerMenu",
            Placeholder = "Select a server to join",
        };

        foreach (var (map, port) in mapsAndPorts)
        {
            serverMenu.AddOption(map, port.ToString(), port.ToString());
        }

        var component = new ComponentBuilder()
            .WithSelectMenu(serverMenu);

        return component;
    }
}
