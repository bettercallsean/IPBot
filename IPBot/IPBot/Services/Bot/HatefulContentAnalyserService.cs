using Discord;
using IPBot.Common.Dtos;
using IPBot.Common.Services;
using IPBot.Constants;
using IPBot.Interfaces.Services;
using Microsoft.IdentityModel.Tokens;

namespace IPBot.Services.Bot;

public class HatefulContentAnalyserService : IHatefulContentAnalyserService
{
    const int HatefulContentSeverityThreshold = 4;

    private readonly IDiscordService _discordService;
    private readonly ILogger<HatefulContentAnalyserService> _logger;
    private readonly IContentAnalyserService _contentAnalyserService;
    private readonly IMessageMediaAnalyserService _messageAnalyserService;
    private readonly ITweetAnalyserService _tweetAnalyserService;

    public HatefulContentAnalyserService(IDiscordService discordService, ILogger<HatefulContentAnalyserService> logger, IContentAnalyserService imageAnalyserService,
        IMessageMediaAnalyserService messageAnalyserService, ITweetAnalyserService tweetAnalyserService)
    {
        _discordService = discordService;
        _logger = logger;
        _contentAnalyserService = imageAnalyserService;
        _messageAnalyserService = messageAnalyserService;
        _tweetAnalyserService = tweetAnalyserService;
    }

    public async Task CheckMessageForHatefulContentAsync(SocketMessage message)
    {
        var user = message.Author as IGuildUser;

        var flaggedUser = await _discordService.GetFlaggedUserAsync(user.Id);
        var userJoined = DateTimeOffset.Now - user.JoinedAt;

        if (flaggedUser is null && userJoined?.Days > BotConstants.NewUserDaysSinceJoinedLimit) return;

        var hatefulContentAnalysis = await GetHatefulContentAnalysisAsync(message);
        var flaggedContentCategories = hatefulContentAnalysis.Where(x => x.Severity >= HatefulContentSeverityThreshold).ToList();

        if (flaggedContentCategories.Count == 0) return;

        var hateCategories = string.Join(", ", flaggedContentCategories.Where(x => x.Severity >= HatefulContentSeverityThreshold).Select(x => x.Category.ToLower()).Distinct());

        await message.DeleteAsync();

        if (flaggedUser is not null)
        {
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

    private async Task<List<CategoryAnalysisDto>> GetHatefulContentAnalysisAsync(SocketMessage message)
    {
        var hatefulContentAnalysis = new List<CategoryAnalysisDto>();

        var hatefulImageContentAnalysis = await GetHatefulImageAnalysisAsync(message);
        var hatefulTextContentAnalysis = await GetHatefulTextAnalysisAsync(message);
        var hatefulTweetContentAnalysis = await GetHatefulTweetAnalysisAsync(message);

        hatefulContentAnalysis.AddRange(hatefulImageContentAnalysis);
        hatefulContentAnalysis.AddRange(hatefulTextContentAnalysis);

        if (hatefulTweetContentAnalysis is not null)
            hatefulContentAnalysis.AddRange(hatefulTweetContentAnalysis);

        return hatefulContentAnalysis;
    }

    private async Task<List<CategoryAnalysisDto>> GetHatefulImageAnalysisAsync(SocketMessage message)
    {
        var contentUrls = await _messageAnalyserService.GetContentUrlsAsync(message);
        if (contentUrls.Count == 0) return [];

        var user = message.Author as IGuildUser;
        if (user is null)
        {
            _logger.LogWarning("Failed to parse message author {AuthorGlobalName}:{AuthorId}", message.Author.GlobalName, message.Author.Id);
            _logger.LogInformation("Checking message from {AuthorGlobalName}:{AuthorId} in {ChannelName} for hateful image content", message.Author.GlobalName, message.Author.Id, message.Channel.Name);
        }
        else
            _logger.LogInformation("Checking message from {User} in {GuildName}:{ChannelName} for hateful text content", user.Username, user.Guild.Name, message.Channel.Name);

        var hatefulContentAnalysis = new List<CategoryAnalysisDto>();
        foreach (var url in contentUrls)
        {
            var encodedUrl = Base64UrlEncoder.Encode(url);

            var analysisDtos = await _contentAnalyserService.GetImageContentSafetyAnalysisAsync(encodedUrl);

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

    private async Task<List<CategoryAnalysisDto>> GetHatefulTextAnalysisAsync(SocketMessage message)
    {
        if (message.Author is not IGuildUser user)
        {
            _logger.LogWarning("Failed to parse message author {AuthorGlobalName}:{AuthorId}", message.Author.GlobalName, message.Author.Id);
            _logger.LogInformation("Checking message from {AuthorGlobalName}:{AuthorId} in {ChannelName} for hateful text content", message.Author.GlobalName, message.Author.Id, message.Channel.Name);
        }
        else
            _logger.LogInformation("Checking message from {User} in {GuildName}:{ChannelName} for hateful text content", user.Username, user.Guild.Name, message.Channel.Name);

        return await _contentAnalyserService.GetTextContentSafetyAnalysisAsync(message.Content);
    }

    private async Task<List<CategoryAnalysisDto>?> GetHatefulTweetAnalysisAsync(SocketMessage message)
    {
        if (!_tweetAnalyserService.ContentContainsTweetLink(message.Content)) return null;

        var tweet = _tweetAnalyserService.GetTweetFromMessageContent(message);
        return await _contentAnalyserService.GetTextContentSafetyAnalysisAsync(tweet.Text);
    }
}
