namespace IPBot.Services;

public class CommandHandler
{
    public static IServiceProvider _provider;
    public static DiscordSocketClient _discord;
    public static CommandService _commands;
    public static IConfigurationRoot _config;
    private Timer _timer;
    private readonly Dictionary<ulong, ulong> _discordChannels;

    public CommandHandler(IServiceProvider provider, DiscordSocketClient discord, CommandService command, IConfigurationRoot config)
    {
        _provider = provider;
        _discord = discord;
        _commands = command;
        _config = config;

        _discordChannels = JsonConvert.DeserializeObject<Dictionary<ulong, ulong>>(File.ReadAllText($"{Constants.ConfigDirectory}/discord_channels.json"));

        _discord.Ready += OnReady;
        _discord.MessageReceived += OnMessageReceived;
    }

    private async Task OnMessageReceived(SocketMessage arg)
    {
        var message = arg as SocketUserMessage;

        if (message.Author.IsBot)
            return;

        var context = new SocketCommandContext(_discord, message);

        int pos = 0;
        if (message.HasStringPrefix(_config["prefix"], ref pos) || message.HasMentionPrefix(_discord.CurrentUser, ref pos))
        {
            var result = await _commands.ExecuteAsync(context, pos, _provider);

            if (!result.IsSuccess)
                Console.WriteLine($"{result.Error}");
        }
    }

    private Task OnReady()
    {
        _timer = new Timer(CheckForUpdatedIP, null, TimeSpan.FromSeconds(1), TimeSpan.FromMinutes(1));

        return Task.CompletedTask;
    }

    private async void CheckForUpdatedIP(object _)
    {
        var ipChangedFile = Path.Combine(Constants.BaseDirectory, "../ip_changed");

        if (!File.Exists(ipChangedFile)) return;

        var ip = await Commands.IPCommands.GetIPFromFile();

        foreach (var (guildId, textChannelId) in _discordChannels)
        {
            var guild = _discord.GetGuild(guildId);
            var channel = guild.GetTextChannel(textChannelId);

            await channel.SendMessageAsync($"Beep boop. The server IP has changed to {ip}");
        }

        File.Delete(ipChangedFile);
    }
}
