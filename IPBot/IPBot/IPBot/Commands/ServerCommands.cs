using Discord;
using IPBot.API.Shared.Services;
using IPBot.Helpers;

namespace IPBot.Commands;

public class ServerCommands : InteractionModuleBase<SocketInteractionContext>
{
    private const string GameServerMenu = "gameServerMenu";

    private readonly IGameService _gameService;
    private readonly IIPService _ipService;

    public ServerCommands(IGameService gameService, IIPService ipService)
    {
        _gameService = gameService;
        _ipService = ipService;
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
            await _gameService.GetMinecraftServerStatusAsync(BotConstants.MinecraftServerPort);

        var serverStatus = ServerInfoHelper.GetServerStatus(serverInfo);

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

        var arkServers = await _gameService.GetActiveServersAsync("Ark");
        var serverStatus = new StringBuilder();
        var activeServers = new Dictionary<string, int>();

        foreach (var server in arkServers)
        {
            var serverInfo = await _gameService.GetSteamServerStatusAsync(server.Port);
            var playerCountStatus = ServerInfoHelper.GetServerStatus(serverInfo);
            var serverMapHasValue = !string.IsNullOrWhiteSpace(serverInfo.Map);

            serverStatus.AppendLine(serverMapHasValue
                    ? $"Map: {serverInfo.Map} - {playerCountStatus} | Port: {server.Port}"
                    : $"{(string.IsNullOrEmpty(server.Map) ? string.Empty : $"Map: {server.Map} - ")}{playerCountStatus} | Port: {server.Port}");

            if (!string.IsNullOrWhiteSpace(serverInfo.Map) && !serverInfo.Map.Equals(server.Map))
            {
                server.Map = serverInfo.Map;
                await _gameService.UpdateGameServerInformationAsync(server);
            }
            
            activeServers.Add(serverInfo.Map, server.Port);
        }

        if (activeServers.Count >= 3)
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
    }

#if DEBUG
    [SlashCommand("zomboid_debug", "get the status of the zomboid server")]
#else
    [SlashCommand("zomboid", "get the status of the zomboid server")]
#endif
    public async Task GetProjectZomboidServerStatusAsync()
    {
        await DeferAsync();

        var serverInfo = await _gameService.GetSteamServerStatusAsync(BotConstants.ZomboidServerPort);

        var serverStatus = ServerInfoHelper.GetServerStatus(serverInfo);

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
        var serverDomain = await _ipService.GetCurrentServerDomainAsync();
        await RespondAsync($"Open up Steam after clicking this link and you should see the 'server connect' menu{Environment.NewLine}" +
            $"steam://connect/{serverDomain}:{inputs[0]}", ephemeral: true);
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
            serverMenu.AddOption(map, port.ToString(), port.ToString());

        var component = new ComponentBuilder()
            .WithSelectMenu(serverMenu);

        return component;
    }
}
