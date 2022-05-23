using IPBot.Helpers;
using IPBot.Infrastructure.Helpers;

namespace IPBot.Services;

public class StartupService
{
    private readonly IConfigurationRoot _config;
    private readonly DiscordSocketClient _discord;
    private readonly InteractionService _commands;
    private readonly MessageAnalyserService _messageAnalyserService;

    public StartupService(DiscordSocketClient discord, InteractionService commands, IConfigurationRoot config, MessageAnalyserService messageAnalyserService)
    {
        _discord = discord;
        _commands = commands;
        _config = config;
        _messageAnalyserService = messageAnalyserService;
    }

    public async Task StartAsync()
    {
        var token = _config["token"];

        await _discord.LoginAsync(Discord.TokenType.Bot, token);
        await _discord.StartAsync();

        _discord.Ready += ReadyAsync;
        _discord.MessageReceived += DiscordOnMessageReceived;
        _discord.Connected += DiscordOnConnected;
    }

    private async Task DiscordOnConnected()
    {
        var serverDomain = await ServerDomainHelper.GetCurrentServerDomainAsync();

        await _discord.SetGameAsync(serverDomain);
    }

    private async Task DiscordOnMessageReceived(SocketMessage message)
    {
        await _messageAnalyserService.CheckMessageForAnimeAsync(message);
    }

    private async Task ReadyAsync()
    {
        if (DebugHelper.IsDebug())
        {
            var guildId = ulong.Parse(_config["testGuild"]);
            await _commands.RegisterCommandsToGuildAsync(guildId);
        }
        else
        {
            await _commands.RegisterCommandsGloballyAsync();
        }
    }
}
