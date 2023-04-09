using System.Reflection;
using Discord;
using Microsoft.Extensions.Logging;

namespace IPBot.Services;

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
    }

    private Task SelectMenuExecuted(SocketMessageComponent arg)
    {
        var user = arg.User as SocketGuildUser;
        _logger.LogInformation("{userName}:{discriminator} ({userId}) clicked menu {menuName} in {guildId} - {channelId}. Selected Values: {selection}",
            user.Username, user.DiscriminatorValue, user.Id, arg.Data.CustomId, user.Guild.Name, arg.Channel.Name, string.Join(",", arg.Data.Values));
        
        return Task.CompletedTask;
    }

    private Task SlashCommandExecuted(SocketSlashCommand arg)
    {
        var user = arg.User as SocketGuildUser;
        _logger.LogInformation("{userName}:{discriminator} ({userId}) called command '{commandName}' in {guildId} - {channelId}",
            user.Username, user.DiscriminatorValue, user.Id, arg.CommandName, user.Guild.Name, arg.Channel.Name);

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
