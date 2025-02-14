using System.Text.RegularExpressions;
using Discord;
using IPBot.Common.Services;
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
        "image/tiff",
        "video/mp4"
    ];

    public async Task CheckMessageForAnimeAsync(SocketMessage message)
    {
        var user = message.Author as IGuildUser;
        var channelIsBeingAnalysedForAnime = await discordService.ChannelIsBeingAnalysedForAnimeAsync(user.Guild.Id, message.Channel.Id);

        if (channelIsBeingAnalysedForAnime && !message.Author.IsBot)
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

        var userIsBeingCheckedForHatefulContent = await discordService.UserIsFlaggedAsync(user.Id);
        var userJoined = DateTimeOffset.Now - user.JoinedAt;

        if (!userIsBeingCheckedForHatefulContent && userJoined?.Days > 90) return;

        logger.LogInformation("Checking message from {User} in channel {GuildName}:{ChannelName} for hateful content", user.Username, user.Guild.Name, message.Channel.Name);

        var messageMediaModel = GetMessageMediaModel(message.Content);
        if (!messageMediaModel.ContainsMedia) return;

        var url = await GetMediaUrlAsync(messageMediaModel, message);
        var encodedUrl = Base64UrlEncoder.Encode(url);
        var hatefulContentAnalysis = await imageAnalyserService.GetContentSafetyAnalysisAsync(encodedUrl);

        if (hatefulContentAnalysis is null)
        {
            logger.LogInformation("Message from {User} in channel {GuildName}:{ChannelName} failed to be analysed for hateful content", user.Username, user.Guild.Name, message.Channel.Name);
            return;
        }

        var flaggedContentCategories = hatefulContentAnalysis.Where(x => x.Severity > 0).ToList();

        if (flaggedContentCategories.Count > 0)
        {
            logger.LogInformation("Message from {User} in channel {GuildName}:{ChannelName} deleted for {Category}", user.Username, user.Guild.Name, message.Channel.Name, string.Join(", ", flaggedContentCategories.Select(x => x.Category)));

            await message.DeleteAsync();
            await discordService.UpdateUserFlaggedCountAsync(user.Id);
        }
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

                var result = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
                if (!_httpImageContentTypes.Contains(result.Content.Headers.ContentType.MediaType))
                    return string.Empty;

                return url;
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