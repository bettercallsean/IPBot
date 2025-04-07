using System.Reflection;
using Discord;

namespace IPBot.Services.Bot;

public class CommandHandler
{
    private readonly ILogger<CommandHandler> _logger;
    private readonly IServiceProvider _services;
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _commands;

    public CommandHandler(ILogger<CommandHandler> logger, IServiceProvider services, DiscordSocketClient client, InteractionService commands)
    {
        _logger = logger;
        _services = services;
        _client = client;
        _commands = commands;
    }

    public async Task InitializeAsync()
    {
        await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

        _client.SlashCommandExecuted += SlashCommandExecuted;
        _client.InteractionCreated += HandleInteraction;
        _client.SelectMenuExecuted += SelectMenuExecuted;
        _client.ButtonExecuted += ButtonExecuted;
    }

    private Task ButtonExecuted(SocketMessageComponent arg)
    {
        var user = arg.User as SocketGuildUser;
        _logger.LogInformation("{UserName} ({UserId}) clicked button {ButtonName} in {GuildName}:{ChannelName}",
            user.Username, user.Id, arg.Data.CustomId, user.Guild.Name, arg.Channel.Name);

        return Task.CompletedTask;
    }

    private Task SelectMenuExecuted(SocketMessageComponent arg)
    {
        var user = arg.User as SocketGuildUser;
        _logger.LogInformation("{UserName} ({UserId}) clicked menu {MenuName} in {GuildName}:{ChannelName}. Selected Values: {Selection}",
            user.Username, user.Id, arg.Data.CustomId, user.Guild.Name, arg.Channel.Name, string.Join(",", arg.Data.Values));

        return Task.CompletedTask;
    }

    private Task SlashCommandExecuted(SocketSlashCommand arg)
    {
        var user = arg.User as SocketGuildUser;
        _logger.LogInformation("{UserName} ({UserId}) called command '{CommandName}' in {GuildName}:{ChannelName}",
            user.Username, user.Id, arg.CommandName, user.Guild.Name, arg.Channel.Name);

        return Task.CompletedTask;
    }

    private async Task HandleInteraction(SocketInteraction arg)
    {
        try
        {
            var ctx = new SocketInteractionContext(_client, arg);
            await _commands.ExecuteCommandAsync(ctx, _services);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling interaction");
            if (arg.Type == InteractionType.ApplicationCommand)
            {
                await arg.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
            }
        }
    }
}
