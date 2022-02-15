using System.Reflection;

namespace IPBot.Services;

public class StartupService
{
    private readonly IServiceProvider _provider;
    private readonly DiscordSocketClient _discord;
    private readonly CommandService _commands;
    private readonly IConfigurationRoot _config;
    private readonly Dictionary<ulong, ulong> _discordChannels;

    public StartupService(IServiceProvider provider, DiscordSocketClient discord, CommandService command, IConfigurationRoot config)
    {
        _provider = provider;
        _discord = discord;
        _commands = command;
        _config = config;

        _discordChannels = JsonConvert.DeserializeObject<Dictionary<ulong, ulong>>(File.ReadAllText($"{Constants.ConfigDirectory}/discord_channels.json"));
    }

    public async Task StartAsync()
    {
        var token = _config["token"];

        await _discord.LoginAsync(Discord.TokenType.Bot, token);
        await _discord.StartAsync();

        await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);

        _discord.Connected += CheckForUpdatedIPAsync;
    }

    private async Task CheckForUpdatedIPAsync()
    {
        var ipChangedFile = Path.Combine(Constants.BaseDirectory, @"../ip_changed");

        if (!File.Exists(ipChangedFile)) return;

        var ip = await Commands.IPCommands.GetIPFromFileAsync();

        foreach (var (guildId, textChannelId) in _discordChannels)
        {
            var guild = _discord.GetGuild(guildId);
            var channel = guild.GetTextChannel(textChannelId);

            await channel.SendMessageAsync($"Beep boop. The server IP has changed to {ip}");
        }

        File.Delete(ipChangedFile);
    }
}
