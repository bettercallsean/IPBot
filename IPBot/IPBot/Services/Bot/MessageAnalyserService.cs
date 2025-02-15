using System.Text.RegularExpressions;
using Discord;
using IPBot.Common.Dtos;
using IPBot.Common.Services;
using IPBot.Helpers;
using IPBot.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace IPBot.Services.Bot;

public partial class MessageAnalyserService(IImageAnalyserService imageAnalyserService, ITenorApiHelper tenorApiHelper,
                                            ILogger<MessageAnalyserService> logger, IDiscordService discordService, HttpClient httpClient)
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

        if (channelIsBeingAnalysedForAnime)
        {
            logger.LogInformation("Checking message from {User} in channel {ChannelName} for anime", user.Username, message.Channel.Name);
            if (await MessageContainsAnimeAsync(message))
            {
                await message.DeleteAsync();
                await message.Channel.SendMessageAsync(_responseList[Random.Shared.Next(_responseList.Count)]);
            }
        }
    }

    public async Task CheckMessageForHatefulContentAsync(SocketMessage message)
    {
        var user = message.Author as IGuildUser;

        var flaggedUser = await discordService.GetFlaggedUserAsync(user.Id);
        var userJoined = DateTimeOffset.Now - user.JoinedAt;

        if (flaggedUser is null && userJoined?.Days > BotConstants.NewUserDaysSinceJoinedLimit) return;

        logger.LogInformation("Checking message from {User} in {GuildName}:{ChannelName} for hateful content", user.Username, user.Guild.Name, message.Channel.Name);

        var hatefulContentAnalysis = await GetHatefulImageAnalysisAsync(message, user);
        var flaggedContentCategories = hatefulContentAnalysis.Where(x => x.Severity > 0).ToList();

        if (flaggedContentCategories.Count > 0)
        {
            logger.LogInformation("Message from {User} in {GuildName}:{ChannelName} deleted for {Category}. They are on strike {StrikeCount}",
                user.Username, user.Guild.Name, message.Channel.Name, string.Join(", ", flaggedContentCategories.Select(x => x.Category)), flaggedUser?.FlaggedCount);

            var guildOwner = await user.Guild.GetOwnerAsync();
            await guildOwner.SendMessageAsync($"Message from {user.Username} in {user.Guild.Name}:{message.Channel.Name} deleted for {string.Join(", ", flaggedContentCategories.Select(x => x.Category))}. " +
                                              $"They are on strike {flaggedUser?.FlaggedCount}");
            await message.DeleteAsync();

            if (flaggedUser is not null)
            {
                if (flaggedUser.FlaggedCount >= BotConstants.MaxHatefulImageFlaggedCount)
                {
                    if (DebugHelper.IsDebug()) return;

                    await user.BanAsync(reason: $"Banned for posting hateful content. Categories: {string.Join(", ", flaggedContentCategories.Select(x => x.Category))}");
                }
                else
                    await discordService.UpdateUserFlaggedCountAsync(user.Id);
            }
            else
            {
                await discordService.CreateFlaggedUserAsync(new()
                {
                    UserId = user.Id,
                    Username = user.Username
                });
            }
        }
    }

    private async Task<List<CategoryAnalysisDto>> GetHatefulImageAnalysisAsync(SocketMessage message, IGuildUser user)
    {
        var messageMediaModel = GetMessageMediaModel(message.Content);

        if (!messageMediaModel.ContainsMedia) return [];

        var url = await GetMediaUrlAsync(messageMediaModel, message);
        var encodedUrl = Base64UrlEncoder.Encode(url);
        var hatefulContentAnalysis = await imageAnalyserService.GetContentSafetyAnalysisAsync(encodedUrl);

        if (hatefulContentAnalysis is not null) return hatefulContentAnalysis;

        logger.LogInformation("Message from {User} in {GuildName}:{ChannelName} failed to be analysed for hateful content", user.Username, user.Guild.Name, message.Channel.Name);
        return [];
    }

    private async Task<string> GetMediaUrlAsync(MessageMediaModel messageMediaModel, SocketMessage message)
    {
        if (!string.IsNullOrEmpty(messageMediaModel.EmojiId))
            return $"https://cdn.discordapp.com/emojis/{messageMediaModel.EmojiId}.png";
        else
        {
            var youtubeUrlModel = MessageContainsYouTubeLink(message.Content);
            if (youtubeUrlModel.ContainsMedia)
                return $"https://i3.ytimg.com/vi/{youtubeUrlModel.Url}/maxresdefault.jpg";
            else
            {
                var url = messageMediaModel.Url;

                if (message.Content.Contains("tenor.com") && !_imageFormats.Any(message.Content.Contains))
                    return await tenorApiHelper.GetDirectTenorGifUrlAsync(url);

                var result = await httpClient.SendAsync(new(HttpMethod.Head, url));

                return !_httpImageContentTypes.Contains(result.Content.Headers.ContentType.MediaType) ? string.Empty : url;
            }
        }
    }

    private async Task<bool> MessageContainsAnimeAsync(SocketMessage message)
    {
        double animeScore;
        if (!string.IsNullOrEmpty(message.Content))
        {
            var messageMediaModel = GetMessageMediaModel(message.Content);
            if (!messageMediaModel.ContainsMedia) return false;

            var url = await GetMediaUrlAsync(messageMediaModel, message);

            if (string.IsNullOrEmpty(url)) return false;

            animeScore = await GetAnimeScoreAsync(url);

            logger.LogInformation("Anime score: {Score}", animeScore);

            if (animeScore >= BotConstants.AnimeScoreTolerance) return true;
        }

        if (message.Attachments.Count <= 0) return false;
        {
            foreach (var attachment in message.Attachments)
            {
                animeScore = await GetAnimeScoreAsync(attachment.ProxyUrl);

                logger.LogInformation("Anime score: {Score}", animeScore);

                if (animeScore < BotConstants.AnimeScoreTolerance) continue;

                return true;
            }
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
            var urlMatch = UrlRegex().Match(word);
            var emojiMatch = DiscordEmojiRegex().Match(word);

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
        var youtubeUrlMatch = YouTubeUrlRegex().Match(messageContent);

        return new()
        {
            ContainsMedia = youtubeUrlMatch.Success,
            Url = youtubeUrlMatch.Success ? youtubeUrlMatch.Groups.Values.ToList()[6].ToString() : string.Empty
        };
    }

    [GeneratedRegex("(https:\\/\\/www\\.|http:\\/\\/www\\.|https:\\/\\/|http:\\/\\/)?[a-zA-Z]{2,}(\\.[a-zA-Z]{2,})(\\.[a-zA-Z]{2,})?\\/[a-zA-Z0-9]{2,}|((https:\\/\\/www\\.|http:\\/\\/www\\.|https:\\/\\/|http:\\/\\/)?[a-zA-Z]{2,}(\\.[a-zA-Z]{2,})(\\.[a-zA-Z]{2,})?)|(https:\\/\\/www\\.|http:\\/\\/www\\.|https:\\/\\/|http:\\/\\/)?[a-zA-Z0-9]{2,}\\.[a-zA-Z0-9]{2,}\\.[a-zA-Z0-9]{2,}(\\.[a-zA-Z0-9]{2,})?")]
    private static partial Regex UrlRegex();

    [GeneratedRegex("<:[a-zA-Z0-9]+:([0-9]+)>")]
    private static partial Regex DiscordEmojiRegex();

    [GeneratedRegex("^((?:https?:)?\\/\\/)?((?:www|m)\\.)?((?:youtube(-nocookie)?\\.com|youtu.be))(\\/(?:[\\w\\-]+\\?v=|embed\\/|v\\/)?)([\\w\\-]+)(\\S+)?$")]
    private static partial Regex YouTubeUrlRegex();
}