namespace IPBot.Services;

public class StartupService
{
    private readonly IConfigurationRoot _config;
    private readonly DiscordSocketClient _discord;
    private readonly InteractionService _commands;
    private readonly Dictionary<ulong, ulong> _discordChannels;

    public StartupService(DiscordSocketClient discord, InteractionService commands, IConfigurationRoot config)
    {
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
        _discord.MessageReceived += DiscordOnMessageReceived;
    }

    private async Task DiscordOnMessageReceived(SocketMessage message)
    {
        var responseList = new List<string>
        {
           "https://c.tenor.com/xwvZutw8Z7AAAAAC/tenor.gif",
           "https://64.media.tumblr.com/c045b0be831f9a3eaff6ef009d182f03/tumblr_mh958xh2jX1qa8a12o1_500.gif",
           "https://i2.wp.com/www.nerdsandbeyond.com/wp-content/uploads/2020/08/SmoggyHilariousBaiji-size_restricted.gif?resize=540%2C304",
           "https://c.tenor.com/NDe_7Jj8RaEAAAAd/tenor.gif"
        };

        var channelName = IsDebug() ? "anti-anime-test" : "the-gospel";

        if (message.Channel.Name == channelName && !message.Author.IsBot)
        {
            if (await MessageAnalyserService.MessageContainsAnimeAsync(message))
            {
                await message.DeleteAsync();
                await message.Channel.SendMessageAsync(responseList.OrderBy(_ => Guid.NewGuid()).Take(1).First());
            }
        }
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
            await _commands.RegisterCommandsGloballyAsync();
        }
    }

    private async Task CheckForUpdatedIPAsync()
    {
        await Task.Delay(1000);

        var ip = await Commands.IPCommands.GetIPFromFileAsync();
        await _discord.SetGameAsync(ip);

        var ipChangedFile = Path.Combine(Constants.BaseDirectory, @"../ip_changed");

        if (!File.Exists(ipChangedFile)) return;

        if (IsDebug())
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

    private static bool IsDebug()
    {
#if DEBUG
        return true;
#else
            return false;
#endif
    }
}
