using System.Reflection;
using Discord;
using Microsoft.Extensions.Logging;

namespace IPBot.Services.Bot;

public class CommandHandler(ILogger<CommandHandler> logger, IServiceProvider services, DiscordSocketClient client, InteractionService commands)
{
    public async Task InitializeAsync()
    {
        await commands.AddModulesAsync(Assembly.GetEntryAssembly(), services);

        client.SlashCommandExecuted += SlashCommandExecuted;
        client.InteractionCreated += HandleInteraction;
        client.SelectMenuExecuted += SelectMenuExecuted;
        client.ButtonExecuted += ButtonExecuted;
    }

    private Task ButtonExecuted(SocketMessageComponent arg)
    {
        var user = arg.User as SocketGuildUser;
        logger.LogInformation("{UserName}:{Discriminator} ({UserId}) clicked button {ButtonName} in {GuildId} - {ChannelId}",
            user.Username, user.DiscriminatorValue, user.Id, arg.Data.CustomId, user.Guild.Name, arg.Channel.Name);

        return Task.CompletedTask;
    }

    private Task SelectMenuExecuted(SocketMessageComponent arg)
    {
        var user = arg.User as SocketGuildUser;
        logger.LogInformation("{UserName}:{Discriminator} ({UserId}) clicked menu {MenuName} in {GuildId} - {ChannelId}. Selected Values: {Selection}",
            user.Username, user.DiscriminatorValue, user.Id, arg.Data.CustomId, user.Guild.Name, arg.Channel.Name, string.Join(",", arg.Data.Values));

        return Task.CompletedTask;
    }

    private Task SlashCommandExecuted(SocketSlashCommand arg)
    {
        var user = arg.User as SocketGuildUser;
        logger.LogInformation("{UserName}:{Discriminator} ({UserId}) called command '{CommandName}' in {GuildId} - {ChannelId}",
            user.Username, user.DiscriminatorValue, user.Id, arg.CommandName, user.Guild.Name, arg.Channel.Name);

        return Task.CompletedTask;
    }

    private async Task HandleInteraction(SocketInteraction arg)
    {
        try
        {
            var ctx = new SocketInteractionContext(client, arg);
            await commands.ExecuteCommandAsync(ctx, services);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling interaction");
            if (arg.Type == InteractionType.ApplicationCommand)
            {
                await arg.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
            }
        }
    }
}
