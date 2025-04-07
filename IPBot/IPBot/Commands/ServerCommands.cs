using IPBot.Common.Dtos;
using IPBot.Common.Services;
using IPBot.Helpers;

namespace IPBot.Commands;

public class ServerCommands : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ILogger<ServerCommands> _logger;
    private readonly IGameService _gameService;
    public ServerCommands(ILogger<ServerCommands> logger, IGameService gameService)
    {
        _logger = logger;
        _gameService = gameService;
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

        var serverStatus = await GetMinecraftServerStatusStringAsync();

        _logger.LogInformation("{ServerStatus}", serverStatus.ToString());

        await FollowupAsync(serverStatus.ToString());
    }

#if DEBUG
    [SlashCommand("ark_debug", "get the status of the ark server")]
#else
    [SlashCommand("ark", "get the status of the ark server")]
#endif
    public async Task GetArkServerStatusAsync()
    {
        const string ArkShortName = "ark";

        _logger.LogInformation("GetArkServerStatusAsync executed");

        await DeferAsync();

        var serverStatus = await GetSteamGameServerStatusStringAsync(ArkShortName);

        if (serverStatus.Split(Environment.NewLine).Length >= 3)
            serverStatus = $"{serverStatus}{Environment.NewLine}Bloody hell, that's a lot of servers 🦖";

        _logger.LogInformation("{ServerStatus}", serverStatus.ToString());
        await FollowupAsync(serverStatus);
    }

#if DEBUG
    [SlashCommand("zomboid_debug", "get the status of the zomboid server")]
#else
    [SlashCommand("zomboid", "get the status of the zomboid server")]
#endif
    public async Task GetProjectZomboidServerStatusAsync()
    {
        const string ProjectZomboidShortName = "zomboid";

        _logger.LogInformation("GetProjectZomboidServerStatusAsync executed");

        await DeferAsync();

        var serverStatus = await GetSteamGameServerStatusStringAsync(ProjectZomboidShortName);

        _logger.LogInformation("{ServerStatus}", serverStatus);
        await FollowupAsync(serverStatus);
    }

    private async Task<string> GetSteamGameServerStatusStringAsync(string gameShortName)
    {
        _logger.LogInformation("Getting list of active servers for {GameShortName}", gameShortName);
        var gameServers = await _gameService.GetActiveServersAsync(gameShortName);

        var serverStatus = new StringBuilder();

        foreach (var gameServer in gameServers)
        {
            _logger.LogInformation("Getting server info for port {Port}", gameServer.Port);

            var serverInfo = await _gameService.GetSteamServerStatusAsync(gameServer.Port);
            var playerCountStatus = ServerInfoHelper.GetServerStatus(serverInfo);
            var serverMapHasValue = !string.IsNullOrWhiteSpace(serverInfo.Map);

            _logger.LogInformation("{Map} - {Port} online: {Online}", gameServer.Map, gameServer.Port, serverInfo.Online);

            serverStatus.AppendLine(serverMapHasValue
                    ? $"Map: {serverInfo.Map} - {playerCountStatus} | Port: {gameServer.Port}"
                    : $"{(string.IsNullOrEmpty(gameServer.Map) ? string.Empty : $"Map: {gameServer.Map} - ")}{playerCountStatus} | Port: {gameServer.Port}");

            if (serverMapHasValue && !serverInfo.Map.Equals(gameServer.Map))
                await UpdateServerMapAsync(serverInfo, gameServer);
        }

        return serverStatus.ToString();
    }

    private async Task<string> GetMinecraftServerStatusStringAsync()
    {
        const string MinecraftShortName = "mc";

        _logger.LogInformation("Getting list of active servers for {ShortName}", MinecraftShortName);
        var gameServers = await _gameService.GetActiveServersAsync(MinecraftShortName);
        var serverStatus = new StringBuilder();

        foreach (var gameServer in gameServers)
        {
            _logger.LogInformation("Getting server info for port {Port}", gameServer.Port);

            var serverInfo = await _gameService.GetMinecraftServerStatusAsync(gameServer.Port);
            var playerCountStatus = ServerInfoHelper.GetServerStatus(serverInfo);

            _logger.LogInformation("{Port} online: {Online}", gameServer.Port, serverInfo.Online);

            serverStatus.AppendLine($"{playerCountStatus} | Port: {gameServer.Port}");
        }

        return serverStatus.ToString();
    }

    private async Task UpdateServerMapAsync(ServerInfoDto serverInfo, GameServerDto gameServer)
    {
        _logger.LogInformation("Updating map information for {Port}. Map updating from {SavedMap} to {NewMap}", gameServer.Port, gameServer.Map, serverInfo.Map);

        gameServer.Map = serverInfo.Map;
        await _gameService.UpdateGameServerInformationAsync(gameServer);
    }
}
