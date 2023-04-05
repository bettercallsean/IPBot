using IPBot.Helpers;
using IPBot.Infrastructure.Helpers;
using Microsoft.Extensions.Logging;

namespace IPBot.Services;

public class StartupService
{
    private readonly ILogger<StartupService> _logger;
    private readonly DiscordSocketClient _discord;
    private readonly InteractionService _commands;

    public StartupService(ILogger<StartupService> logger, DiscordSocketClient discord, InteractionService commands, MessageAnalyserService messageAnalyserService)
    {
        _logger = logger;
        _discord = discord;
        _commands = commands;
    }

    public async Task StartAsync()
    {
        _logger.LogInformation("Starting...");

        var token = DotEnvHelper.EnvironmentVariables["TOKEN"];

        await _discord.LoginAsync(Discord.TokenType.Bot, token);
        await _discord.StartAsync();

        _discord.Ready += ReadyAsync;
        _discord.Connected += DiscordOnConnected;
    }

    private async Task DiscordOnConnected()
    {
        _logger.LogInformation("Connected");

        var serverDomain = ServerDomainHelper.GetCurrentServerDomain();

        await _discord.SetGameAsync(serverDomain);
    }

    private async Task ReadyAsync()
    {
        if (DebugHelper.IsDebug())
        {
            var guildId = ulong.Parse(DotEnvHelper.EnvironmentVariables["TEST_GUILD"]);
            await _commands.RegisterCommandsToGuildAsync(guildId);
        }
        else
        {
            await _commands.RegisterCommandsGloballyAsync();
        }
    }
}
