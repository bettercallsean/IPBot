using IPBot.Helpers;
using IPBot.Infrastructure.Interfaces;

namespace IPBot.Services;

public class StartupService
{
    private readonly IConfigurationRoot _config;
    private readonly DiscordSocketClient _discord;
    private readonly InteractionService _commands;
    private readonly MessageAnalyserService _messageAnalyserService;
    private readonly IIPService _ipService;
    private readonly Dictionary<ulong, ulong> _discordChannels;

    public StartupService(DiscordSocketClient discord, InteractionService commands, IConfigurationRoot config, MessageAnalyserService messageAnalyserService,
        IIPService ipService)
    {
        _discord = discord;
        _commands = commands;
        _config = config;
        _messageAnalyserService = messageAnalyserService;
        _ipService = ipService;
        
        _discordChannels = JsonConvert.DeserializeObject<Dictionary<ulong, ulong>>(File.ReadAllText($"{Constants.ConfigDirectory}/discord_channels.json"));
    }

    public async Task StartAsync()
    {
        var token = _config["token"];

        await _discord.LoginAsync(Discord.TokenType.Bot, token);
        await _discord.StartAsync();

        _discord.Connected += CheckForUpdatedIPAsync;
        _discord.Ready += ReadyAsync;
        _discord.MessageReceived += DiscordOnMessageReceived;
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

    private async Task CheckForUpdatedIPAsync()
    {
        await Task.Delay(1000);

        var ip = await _ipService.GetCurrentIPAsync();
        await _discord.SetGameAsync(ip);

        var ipChangedFile = Path.Combine(Constants.BaseDirectory, @"../ip_changed");

        if (!File.Exists(ipChangedFile)) return;

        if (DebugHelper.IsDebug())
        {
            var testGuildId = ulong.Parse(_config["testGuild"]);
            var testChannelId = ulong.Parse(_config["testGuildTextChannel"]);
            
            var guild = _discord.GetGuild(testGuildId);
            var channel = guild.GetTextChannel(testChannelId);
            
            await channel.SendMessageAsync($"⚠ Beep boop. The server IP has changed to {ip} ⚠");
        }
        else
        {
            foreach (var (guildId, textChannelId) in _discordChannels)
            {
                var guild = _discord.GetGuild(guildId);
                var channel = guild.GetTextChannel(textChannelId);

                await channel.SendMessageAsync($"⚠ Beep boop. The server IP has changed to {ip} ⚠");
            }
            
            File.Delete(ipChangedFile);
        }
    }
}
