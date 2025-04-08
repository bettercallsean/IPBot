using Discord;
using IPBot.Common.Dtos;
using IPBot.Common.Services;
using IPBot.Constants;
using IPBot.Helpers;
using IPBot.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace IPBot.Services.Bot;

public class HatefulContentAnalyserService : IHatefulContentAnalyserService
{
    private readonly IDiscordService _discordService;
    private readonly ILogger<HatefulContentAnalyserService> _logger;
    private readonly IImageAnalyserService _imageAnalyserService;
    private readonly IMessageMediaAnalyserService _messageAnalyserService;

    public HatefulContentAnalyserService(IDiscordService discordService, ILogger<HatefulContentAnalyserService> logger, IImageAnalyserService imageAnalyserService, IMessageMediaAnalyserService messageAnalyserService)
    {
        _discordService = discordService;
        _logger = logger;
        _imageAnalyserService = imageAnalyserService;
        _messageAnalyserService = messageAnalyserService;
    }

    public async Task CheckMessageForHatefulContentAsync(SocketMessage message)
    {
        var user = message.Author as IGuildUser;

        var flaggedUser = await _discordService.GetFlaggedUserAsync(user.Id);
        var userJoined = DateTimeOffset.Now - user.JoinedAt;

        if (flaggedUser is null && userJoined?.Days > BotConstants.NewUserDaysSinceJoinedLimit) return;

        var hatefulContentAnalysis = await GetHatefulImageAnalysisAsync(message);
        var flaggedContentCategories = hatefulContentAnalysis.Where(x => x.Severity > 0).ToList();

        if (flaggedContentCategories.Count == 0) return;

        var hateCategories = string.Join(", ", flaggedContentCategories.Select(x => x.Category).Distinct());

        await message.DeleteAsync();

        if (flaggedUser is not null)
        {
            if (flaggedUser.FlaggedCount >= BotConstants.MaxHatefulImageFlaggedCount && !DebugHelper.IsDebug())
                await user.BanAsync(reason: $"Banned for posting hateful content. Categories: {hateCategories}");
            else
                await _discordService.UpdateUserFlaggedCountAsync(user.Id);
        }
        else
        {
            await _discordService.CreateFlaggedUserAsync(new()
            {
                UserId = user.Id,
                Username = user.Username,
                FlaggedCount = 1
            });
        }

        flaggedUser = await _discordService.GetFlaggedUserAsync(user.Id);

        var guildOwner = await user.Guild.GetOwnerAsync();
        await guildOwner.SendMessageAsync($"Message from {user.Username} in {user.Guild.Name}:{message.Channel.Name} deleted for {hateCategories}. " +
                                          $"They are on strike {flaggedUser?.FlaggedCount}");

        _logger.LogInformation("Message from {User} in {GuildName}:{ChannelName} deleted for {Category}. They are on strike {StrikeCount}",
            user.Username, user.Guild.Name, message.Channel.Name, hateCategories, flaggedUser?.FlaggedCount);
    }

    private async Task<List<CategoryAnalysisDto>> GetHatefulImageAnalysisAsync(SocketMessage message)
    {
        var contentUrls = await _messageAnalyserService.GetContentUrlsAsync(message);
        if (contentUrls.Count == 0) return [];

        var user = message.Author as IGuildUser;
        _logger.LogInformation("Checking message from {User} in {GuildName}:{ChannelName} for hateful content", user.Username, user.Guild.Name, message.Channel.Name);

        var hatefulContentAnalysis = new List<CategoryAnalysisDto>();
        foreach (var url in contentUrls)
        {
            var encodedUrl = Base64UrlEncoder.Encode(url);

            var analysisDtos = await _imageAnalyserService.GetContentSafetyAnalysisAsync(encodedUrl);

            if (analysisDtos is not null)
            {
                _logger.LogInformation("{ContentUrl} analysed", url);
                hatefulContentAnalysis.AddRange(analysisDtos);
            }
            else
                _logger.LogError("{ContentUrl} in {GuildName}:{ChannelName} failed to be analysed for hateful content", url, user.Guild.Name, message.Channel.Name);
        }

        return hatefulContentAnalysis;
    }
}
