using Discord;
using IPBot.Common.Dtos;
using IPBot.Common.Services;

namespace IPBot.Commands;

public class DiscordCommands(ILogger<IPCommands> logger, IDiscordService discordService) : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ILogger<IPCommands> _logger = logger;
    private readonly IDiscordService _discordService = discordService;

#if DEBUG
    [SlashCommand("flag_user_debug", "flag user for hateful content analysis")]
#else
    [SlashCommand("flag_user", "flag user for hateful content analysis")]
#endif
    [DefaultMemberPermissions(GuildPermission.Administrator)]
    protected async Task FlagUserAsync([Summary("user", "user that you want to be flagged")] IGuildUser user)
    {
        await DeferAsync(ephemeral: true);

        _logger.LogInformation("FlagUserAsync executed");

        var userAdded = await _discordService.CreateFlaggedUserAsync(new FlaggedUserDto
        {
            UserId = user.Id,
            Username = user.Username
        });

        if (userAdded)
            await FollowupAsync($"User {user.Username} added to flagged user list", ephemeral: true);
        else
            await FollowupAsync($"Failed to add user {user.Username} to flagged user list. Check to see if they've been added already.", ephemeral: true);
    }

#if DEBUG
    [SlashCommand("delete_flagged_user_debug", "delete flagged user from hateful content analysis list")]
#else
    [SlashCommand("delete_flagged_user", "delete flagged user from hateful content analysis list")]
#endif
    [DefaultMemberPermissions(GuildPermission.Administrator)]
    protected async Task DeleteFlaggedUserAsync([Summary("user", "user that you want to remove from list")] IGuildUser user)
    {
        await DeferAsync(ephemeral: true);

        _logger.LogInformation("DeleteFlaggedUserAsync executed");

        var userDeleted = await _discordService.DeleteFlaggedUserAsync(user.Id);

        if (userDeleted)
            await FollowupAsync($"User {user.Username} removed from flagged user list", ephemeral: true);
        else
            await FollowupAsync($"Failed to remove user {user.Username} from flagged user list. Please try again in a few minutes.", ephemeral: true);
    }

#if DEBUG
    [SlashCommand("get_flagged_users_debug", "get list of flagged users")]
#else
    [SlashCommand("get_flagged_users", "get list of flagged users")]
#endif
    [DefaultMemberPermissions(GuildPermission.Administrator)]
    protected async Task GetFlaggedUsersAsync()
    {
        await DeferAsync(ephemeral: true);

        _logger.LogInformation("GetFlaggedUsersAsync executed");

        var flaggedUsers = await _discordService.GetFlaggedUsersAsync();

        if (flaggedUsers.Count == 0)
            await FollowupAsync("No flagged users!");
        else
            await FollowupAsync(string.Join($"{Environment.NewLine}", flaggedUsers.Select(x => $"User: {x.Username} | Flagged Count: {x.FlaggedCount}")));
    }

#if DEBUG
    [SlashCommand("toggle_tweet_scanning_debug", "enable/disable twitter link scanning")]
#else
    [SlashCommand("toggle_tweet_scanning", "enable/disable twitter link scanning")]
#endif
    [DefaultMemberPermissions(GuildPermission.Administrator)]
    [RequireContext(ContextType.Guild)]
    protected async Task ToggleTwitterLinkScanningAsync()
    {
        _logger.LogInformation("ToggleTwitterLinkScanningAsync executed");

        var guild = await _discordService.GetDiscordGuildAsync(Context.Guild.Id);

        guild.CheckForTwitterLinks = !guild.CheckForTwitterLinks;
        var toggledSuccessfully = await _discordService.UpdateDiscordGuild(Context.Guild.Id, guild);

        if (toggledSuccessfully)
            await RespondAsync($"Tweet scanning has been toggled {(guild.CheckForTwitterLinks ? "off" : "on")}", ephemeral: true);
        else
            await RespondAsync($"Failed to toggle tweet scanning {(guild.CheckForTwitterLinks ? "off" : "on")}, please trying again later", ephemeral: true);
    }
}
