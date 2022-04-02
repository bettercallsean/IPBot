using System.Threading;

namespace IPBot.Services;

public class StartupService
{
    private readonly IServiceProvider _provider;
    private readonly IConfigurationRoot _config;
    private readonly DiscordSocketClient _discord;
    private readonly InteractionService _commands;
    private readonly Dictionary<ulong, ulong> _discordChannels;

    public StartupService(IServiceProvider provider, DiscordSocketClient discord, InteractionService commands, IConfigurationRoot config)
    {
        _provider = provider;
        _discord = discord;
        _commands = commands;
        _config = config;

        _discordChannels = JsonConvert.DeserializeObject<Dictionary<ulong, ulong>>(File.ReadAllText($"{Constants.ConfigDirectory}/discord_channels.json"));
    }

    public async Task StartAsync()
    {
        var token = _config["token"];

        await _discord.LoginAsync(Discord.TokenType.Bot, token);
        await _discord.StartAsync();

        _discord.Connected += CheckForUpdatedIPAsync;
        _discord.Ready += ReadyAsync;
    }

    private async Task ReadyAsync()
    {
        if (IsDebug())
        {
            var guildId = ulong.Parse(_config["testGuild"]);
            await _commands.RegisterCommandsToGuildAsync(guildId);
        }
        else
        {
            await _commands.RegisterCommandsGloballyAsync(true);
        }
    }

    private async Task CheckForUpdatedIPAsync()
    {
        Thread.Sleep(1000);

        var ip = await Commands.IPCommands.GetIPFromFileAsync();
        await _discord.SetGameAsync(ip);

        var ipChangedFile = Path.Combine(Constants.BaseDirectory, @"../ip_changed");

        if (!File.Exists(ipChangedFile)) return;

        foreach (var (guildId, textChannelId) in _discordChannels)
        {
            var guild = _discord.GetGuild(guildId);
            var channel = guild.GetTextChannel(textChannelId);

            await channel.SendMessageAsync($"Beep boop. The server IP has changed to {ip}");
        }

        File.Delete(ipChangedFile);
    }

    private static bool IsDebug()
    {
#if DEBUG
        return true;
#else
            return false;
#endif
    }
}
