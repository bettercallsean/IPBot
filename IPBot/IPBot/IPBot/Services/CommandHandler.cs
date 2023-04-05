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
    }

    private Task SlashCommandExecuted(SocketSlashCommand arg)
    {
        return Task.Run(() =>
        {
            var user = arg.User as SocketGuildUser;
            _logger.LogInformation("{userName}:{discriminator} ({userId}) called {commandName} in Guild {guildId} in Channel {channelId}", user.Username, user.DiscriminatorValue, user.Id, arg.CommandName, user.Guild.Name, arg.Channel.Name);
        });
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
            Console.WriteLine(ex);
            if (arg.Type == InteractionType.ApplicationCommand)
            {
                await arg.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
            }
        }
    }
}
