using Discord;
using IPBot.Common.Dtos;
using IPBot.Common.Services;
using IPBot.Helpers;
using IPBot.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace IPBot.Services.Bot;

public class MessageAnalyserService(IImageAnalyserService imageAnalyserService, ITenorApiHelper tenorApiHelper,
                                            ILogger<MessageAnalyserService> logger, IDiscordService discordService, HttpClient httpClient, ITweetService tweetService)
{
    private readonly List<string> _responseList = [.. Resources.Resources.ResponseGifs.Split(Environment.NewLine)];
    private readonly List<string> _imageFormats =
    [
        ".png",
        ".jpeg",
        ".jpg",
        ".tiff",
        ".mp4",
        ".gif"
    ];

    private readonly List<string> _httpImageContentTypes =
    [
        "image/gif",
        "image/jpeg",
        "image/png",
        "image/tiff"
    ];

    public async Task CheckMessageForAnimeAsync(SocketMessage message)
    {
        var user = message.Author as IGuildUser;
        var channelIsBeingAnalysedForAnime = await discordService.ChannelIsBeingAnalysedForAnimeAsync(user.Guild.Id, message.Channel.Id);

        if (!channelIsBeingAnalysedForAnime) return;

        if (await MessageContainsAnimeAsync(message))
        {
            await message.DeleteAsync();
            await message.Channel.SendMessageAsync(_responseList[Random.Shared.Next(_responseList.Count)]);
        }
    }

    public async Task CheckMessageForHatefulContentAsync(SocketMessage message)
    {
        var user = message.Author as IGuildUser;

        var flaggedUser = await discordService.GetFlaggedUserAsync(user.Id);
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
                await discordService.UpdateUserFlaggedCountAsync(user.Id);
        }
        else
        {
            await discordService.CreateFlaggedUserAsync(new()
            {
                UserId = user.Id,
                Username = user.Username,
                FlaggedCount = 1
            });
        }

        flaggedUser = await discordService.GetFlaggedUserAsync(user.Id);

        var guildOwner = await user.Guild.GetOwnerAsync();
        await guildOwner.SendMessageAsync($"Message from {user.Username} in {user.Guild.Name}:{message.Channel.Name} deleted for {hateCategories}. " +
                                          $"They are on strike {flaggedUser?.FlaggedCount}");

        logger.LogInformation("Message from {User} in {GuildName}:{ChannelName} deleted for {Category}. They are on strike {StrikeCount}",
            user.Username, user.Guild.Name, message.Channel.Name, hateCategories, flaggedUser?.FlaggedCount);
    }

    public async Task CheckForTwitterLinksAsync(SocketMessage message)
    {
        logger.LogInformation("Checking message for twitter links");
        
        var channel = message.Channel as SocketGuildChannel;
        if (channel != null && channel.Guild.Name != "BetterCallSean's Bot Junkyard") return;

        if (tweetService.ContentContainsTweetLink(message.Content) && message is IUserMessage userMessage)
        {
            var extractedLink = tweetService.GetFixUpXLink(message.Content);
            var tweetImageLink = await tweetService.GetDirectTweetImageLinkAsync(message.Content);

            if (string.IsNullOrEmpty(tweetImageLink)) return;
            
            logger.LogInformation("Responding to {Username} in {GuildName}:{ChannelName} with fixed link {Url}", message.Author.Username, channel.Guild.Name, channel.Name, extractedLink);
            
            await userMessage.ReplyAsync(extractedLink);
        }
    }

    private async Task<List<CategoryAnalysisDto>> GetHatefulImageAnalysisAsync(SocketMessage message)
    {
        var contentUrls = await GetContentUrlsAsync(message);
        if (contentUrls.Count == 0) return [];

        var user = message.Author as IGuildUser;
        logger.LogInformation("Checking message from {User} in {GuildName}:{ChannelName} for hateful content", user.Username, user.Guild.Name, message.Channel.Name);

        var hatefulContentAnalysis = new List<CategoryAnalysisDto>();
        foreach (var url in contentUrls)
        {
            var encodedUrl = Base64UrlEncoder.Encode(url);

            var analysisDtos = await imageAnalyserService.GetContentSafetyAnalysisAsync(encodedUrl);

            if (analysisDtos is not null)
            {
                logger.LogInformation("{ContentUrl} analysed", url);
                hatefulContentAnalysis.AddRange(analysisDtos);
            }
            else
                logger.LogError("{ContentUrl} in {GuildName}:{ChannelName} failed to be analysed for hateful content", url, user.Guild.Name, message.Channel.Name);
        }

        return hatefulContentAnalysis;
    }

    private async Task<string> GetMediaUrlAsync(MessageMediaModel messageMediaModel, SocketMessage message)
    {
        if (!string.IsNullOrEmpty(messageMediaModel.EmojiId))
            return $"https://cdn.discordapp.com/emojis/{messageMediaModel.EmojiId}.png";
        
        var youtubeUrlModel = MessageContainsYouTubeLink(message.Content);
        if (youtubeUrlModel.ContainsMedia)
            return $"https://i3.ytimg.com/vi/{youtubeUrlModel.Url}/maxresdefault.jpg";
            
        var url = messageMediaModel.Url;

        if (message.Content.Contains("tenor.com") && !_imageFormats.Any(message.Content.Contains))
            return await tenorApiHelper.GetDirectTenorGifUrlAsync(url);

        if (url.Contains("https://x.com"))
            return await tweetService.GetDirectTweetImageLinkAsync(url);

        var result = await httpClient.SendAsync(new(HttpMethod.Head, url));

        return _httpImageContentTypes.Contains(result.Content.Headers.ContentType?.MediaType) ? url : string.Empty;
    }

    private async Task<List<string>> GetContentUrlsAsync(SocketMessage message)
    {
        var contentUrls = new List<string>();

        if (!string.IsNullOrEmpty(message.Content))
        {
            var messageMediaModel = GetMessageMediaModel(message.Content);
            if (messageMediaModel.ContainsMedia)
            {
                var url = await GetMediaUrlAsync(messageMediaModel, message);

                if (!string.IsNullOrEmpty(url))
                    contentUrls.Add(url);
            }
        }

        if (message.Attachments.Count > 0)
            contentUrls.AddRange(message.Attachments.Select(x => x.ProxyUrl));

        return contentUrls;
    }

    private async Task<bool> MessageContainsAnimeAsync(SocketMessage message)
    {
        var contentUrls = await GetContentUrlsAsync(message);

        if (contentUrls.Count == 0) return false;

        var user = message.Author as IGuildUser;
        logger.LogInformation("Checking message from {User} in {GuildName}:{ChannelName} for anime", user.Username, user.Guild.Name, message.Channel.Name);

        foreach (var url in contentUrls)
        {
            var animeScore = await GetAnimeScoreAsync(url);

            logger.LogInformation("Anime score: {Score}", animeScore);

            if (animeScore < BotConstants.AnimeScoreTolerance) continue;

            return true;
        }

        return false;
    }

    private async Task<double> GetAnimeScoreAsync(string url)
    {
        var encodedUrl = Base64UrlEncoder.Encode(url);
        return await imageAnalyserService.GetAnimeScoreAsync(encodedUrl);
    }

    private static MessageMediaModel GetMessageMediaModel(string messageContent)
    {
        foreach (var word in messageContent.Split())
        {
            var urlMatch = RegexHelper.UrlRegex().Match(word);
            var emojiMatch = RegexHelper.DiscordEmojiRegex().Match(word);

            if (!urlMatch.Success && !emojiMatch.Success) continue;

            return new()
            {
                ContainsMedia = true,
                Url = word,
                EmojiId = emojiMatch.Success ? emojiMatch.Groups.Values.ToArray()[1].ToString() : string.Empty
            };
        }

        return new()
        {
            ContainsMedia = false
        };
    }

    private static MessageMediaModel MessageContainsYouTubeLink(string messageContent)
    {
        var youtubeUrlMatch = RegexHelper.YouTubeUrlRegex().Match(messageContent);

        return new()
        {
            ContainsMedia = youtubeUrlMatch.Success,
            Url = youtubeUrlMatch.Success ? youtubeUrlMatch.Groups.Values.ToList()[6].ToString() : string.Empty
        };
    }


}