using IPBot.Helpers;
using IPBot.Infrastructure.Helpers;

namespace IPBot.Services;

public class StartupService
{
    private readonly DiscordSocketClient _discord;
    private readonly InteractionService _commands;
    private readonly MessageAnalyserService _messageAnalyserService;

    public StartupService(DiscordSocketClient discord, InteractionService commands, MessageAnalyserService messageAnalyserService)
    {
        _discord = discord;
        _commands = commands;
        _messageAnalyserService = messageAnalyserService;
    }

    public async Task StartAsync()
    {
        var token = DotEnvHelper.EnvironmentVariables["TOKEN"];

        await _discord.LoginAsync(Discord.TokenType.Bot, token);
        await _discord.StartAsync();

        _discord.Ready += ReadyAsync;
        _discord.Connected += DiscordOnConnected;
    }

    private async Task DiscordOnConnected()
    {
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
