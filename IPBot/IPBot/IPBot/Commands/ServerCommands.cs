﻿using Discord;
using IPBot.Common.Dtos;
using IPBot.Common.Services;
using IPBot.Helpers;
using Microsoft.Extensions.Logging;

namespace IPBot.Commands;

public class ServerCommands : InteractionModuleBase<SocketInteractionContext>
{
    private const string GameServerMenu = "gameServerMenu";
    private const string GameServerButton = "minecraftServerButton";

    private readonly ILogger<ServerCommands> _logger;
    private readonly IGameService _gameService;
    private readonly IIPService _ipService;

    public ServerCommands(ILogger<ServerCommands> logger, IGameService gameService, IIPService ipService)
    {
        _logger = logger;
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
        _logger.LogInformation("GetMinecraftServerStatusAsync executed");

        await DeferAsync();

        var serverInfo =
            await _gameService.GetMinecraftServerStatusAsync(BotConstants.MinecraftServerPort);

        _logger.LogInformation("{Port} online: {Online}", BotConstants.MinecraftServerPort, serverInfo.Online);

        var serverStatus = ServerInfoHelper.GetServerStatus(serverInfo);

        _logger.LogInformation("{ServerStatus}", serverStatus);

        if (serverInfo.PlayerCount > 0)
        {
            _logger.LogInformation("Building component for Minecraft server");

            var button = CreateMinecraftButtonComponent();
            await FollowupAsync(serverStatus, components: button.Build());
        }
        else
        {
            await FollowupAsync(serverStatus);
        }

    }

#if DEBUG
    [SlashCommand("ark_debug", "get the status of the ark server")]
#else
    [SlashCommand("ark", "get the status of the ark server")]
#endif
    public async Task GetArkServerStatusAsync()
    {
        _logger.LogInformation("GetArkServerStatusAsync executed");

        await DeferAsync();

        _logger.LogInformation("Getting list of active servers");
        var arkServers = await _gameService.GetActiveServersAsync("Ark");
        var serverStatus = new StringBuilder();
        var activeServers = new Dictionary<string, int>();

        foreach (var server in arkServers)
        {
            _logger.LogInformation("Getting server info for port {Port}", server.Port);

            var serverInfo = await _gameService.GetSteamServerStatusAsync(server.Port);
            var playerCountStatus = ServerInfoHelper.GetServerStatus(serverInfo);
            var serverMapHasValue = !string.IsNullOrWhiteSpace(serverInfo.Map);

            _logger.LogInformation("{Map} - {Port} online: {Online}", server.Map, server.Port, serverInfo.Online);

            serverStatus.AppendLine(serverMapHasValue
                    ? $"Map: {serverInfo.Map} - {playerCountStatus} | Port: {server.Port}"
                    : $"{(string.IsNullOrEmpty(server.Map) ? string.Empty : $"Map: {server.Map} - ")}{playerCountStatus} | Port: {server.Port}");

            if (!string.IsNullOrWhiteSpace(serverInfo.Map) && !serverInfo.Map.Equals(server.Map))
            {
                _logger.LogInformation("Updating map information for {Port}. Map updating from {SavedMap} to {NewMap}", server.Port, server.Map, serverInfo.Map);

                server.Map = serverInfo.Map;
                await _gameService.UpdateGameServerInformationAsync(server);
            }

            if (serverInfo.Online)
                activeServers.Add(serverInfo.Map, server.Port);
        }

        if (activeServers.Count >= 3)
            serverStatus.AppendLine($"{Environment.NewLine}Bloody hell, that's a lot of servers 🦖");

        var serverStatusMessage = serverStatus.ToString();

        if (activeServers.Any())
        {
            _logger.LogInformation("Building component for {ActiveServerCount} server(s)", activeServers.Count);
            var component = CreateGameServerMenuComponent(activeServers);

            await FollowupAsync(serverStatusMessage, components: component.Build());
        }
        else
        {
            await FollowupAsync(serverStatusMessage);
        }

        _logger.LogInformation("{ServerStatusMessage}", serverStatusMessage);
    }

#if DEBUG
    [SlashCommand("zomboid_debug", "get the status of the zomboid server")]
#else
    [SlashCommand("zomboid", "get the status of the zomboid server")]
#endif
    public async Task GetProjectZomboidServerStatusAsync()
    {
        _logger.LogInformation("GetProjectZomboidServerStatusAsync executed");

        await DeferAsync();

        var serverInfo = await _gameService.GetSteamServerStatusAsync(BotConstants.ZomboidServerPort);

        _logger.LogInformation("{Map} - {Port} online: {Online}", serverInfo.Map, BotConstants.ZomboidServerPort, serverInfo.Online);

        var serverStatus = ServerInfoHelper.GetServerStatus(serverInfo);

        if (serverInfo.Online)
        {
            _logger.LogInformation("Building component for {Port}", BotConstants.ZomboidServerPort);
            var component = CreateGameServerMenuComponent(serverInfo.Map, BotConstants.ZomboidServerPort);

            _logger.LogInformation("{ServerStatus}", serverStatus);
            await FollowupAsync(serverStatus, components: component.Build());
        }
        else
        {
            _logger.LogInformation("{ServerStatus}", serverStatus);
            await FollowupAsync(serverStatus);
        }
    }

    [ComponentInteraction(GameServerMenu)]
    public async Task GenerateSteamConnectLinkAsync(string[] inputs)
    {
        _logger.LogInformation("GenerateSteamConnectLinkAsync executed");

        var serverDomain = await _ipService.GetCurrentServerDomainAsync();
        var serverInfo = await GetServerInfoStringAsync("steam", int.Parse(inputs[0]));

        await RespondAsync($"{serverInfo}{Environment.NewLine}{Environment.NewLine}" +
            $"Open up Steam after clicking this link and you should see the 'server connect' menu{Environment.NewLine}" +
            $"steam://connect/{serverDomain}:{inputs[0]}", ephemeral: true);
    }

    [ComponentInteraction(GameServerButton)]
    public async Task GenerateMinecraftInfoAsync()
    {
        _logger.LogInformation("GenerateMinecraftInfoAsync executed");

        var serverInfoString = await GetServerInfoStringAsync("mc", BotConstants.MinecraftServerPort);

        await RespondAsync(serverInfoString, ephemeral: true);

        _logger.LogInformation("GenerateMinecraftInfoAsync responded");
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
            CustomId = GameServerMenu,
            Placeholder = "Select a server to join",
        };

        foreach (var (map, port) in mapsAndPorts)
            serverMenu.AddOption(map, port.ToString(), port.ToString());

        var component = new ComponentBuilder()
            .WithSelectMenu(serverMenu);

        return component;
    }

    private static ComponentBuilder CreateMinecraftButtonComponent()
    {
        const string pigEmoji = "\uD83D\uDC37";

        var serverMenu = new ButtonBuilder
        {
            CustomId = GameServerButton,
            Label = "More info",
            Style = ButtonStyle.Primary,
            Emote = new Emoji(pigEmoji)
        };

        var component = new ComponentBuilder()
            .WithButton(serverMenu);

        return component;
    }

    private async Task<string> GetServerInfoStringAsync(string gameCode, int port)
    {
        ServerInfoDto serverInfo;
        if (gameCode == "mc")
            serverInfo = await _gameService.GetMinecraftServerStatusAsync(port);
        else
            serverInfo = await _gameService.GetSteamServerStatusAsync(port);

        var playerInfo = new StringBuilder();

        playerInfo.Append($"{Environment.NewLine}Players online: {(serverInfo.PlayerNames.Count > 0
            ? string.Join(", ", serverInfo.PlayerNames)
            : serverInfo.PlayerCount)}");

        if (!string.IsNullOrWhiteSpace(serverInfo.Motd))
            playerInfo.Append($"{Environment.NewLine}MOTD: {serverInfo.Motd}");

        return playerInfo.ToString();
    }
}
