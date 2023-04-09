using IPBot.Helpers;
using IPBot.Shared.Services;
using Microsoft.Extensions.Logging;

namespace IPBot.Services;

public class StartupService
{
    private readonly ILogger<StartupService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IIPService _ipService;
    private readonly DiscordSocketClient _discord;
    private readonly InteractionService _commands;

    public StartupService(ILogger<StartupService> logger, IConfiguration configuration, IIPService ipService, DiscordSocketClient discord, InteractionService commands)
    {
        _logger = logger;
        _configuration = configuration;
        _ipService = ipService;
        _discord = discord;
        _commands = commands;
    }

    public async Task StartAsync()
    {
        _logger.LogInformation("Starting...");

        var token = _configuration["BotToken"];

        await _discord.LoginAsync(Discord.TokenType.Bot, token);
        await _discord.StartAsync();

        _discord.Ready += ReadyAsync;
        _discord.Connected += DiscordOnConnected;
    }

    private async Task DiscordOnConnected()
    {
        _logger.LogInformation("Connected");

        var serverDomain = await _ipService.GetCurrentServerDomainAsync();
        _logger.LogInformation("Domain: {serverDomain}", serverDomain);

        await _discord.SetGameAsync(serverDomain);
    }

    private async Task ReadyAsync()
    {
        if (DebugHelper.IsDebug())
        {
            var guildId = ulong.Parse(_configuration["TestGuild"]);
            await _commands.RegisterCommandsToGuildAsync(guildId);
        }
        else
        {
            await _commands.RegisterCommandsGloballyAsync();
        }
    }
}
