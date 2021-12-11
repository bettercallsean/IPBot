namespace IPBot.Services;

public class CommandHandler
{
    private readonly IServiceProvider _provider;
    private readonly DiscordSocketClient _discord;
    private readonly CommandService _commands;
    private readonly IConfigurationRoot _config;

    public CommandHandler(IServiceProvider provider, DiscordSocketClient discord, CommandService command, IConfigurationRoot config)
    {
        _provider = provider;
        _discord = discord;
        _commands = command;
        _config = config;


        _discord.MessageReceived += OnMessageReceivedAsync;
    }

    private async Task OnMessageReceivedAsync(SocketMessage arg)
    {
        var message = arg as SocketUserMessage;

        if (message.Author.IsBot) return;


        var context = new SocketCommandContext(_discord, message);

        int pos = 0;
        if (message.HasStringPrefix(_config["prefix"], ref pos) || message.HasMentionPrefix(_discord.CurrentUser, ref pos))
        {
            var result = await _commands.ExecuteAsync(context, pos, _provider);

            if (!result.IsSuccess)
            {
                Console.WriteLine($"{result.Error}");
            }
        }
    }
}
