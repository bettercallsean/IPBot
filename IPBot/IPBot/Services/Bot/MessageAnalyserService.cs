using System.Text.RegularExpressions;
using IPBot.Common.Services;
using IPBot.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace IPBot.Services;

public partial class MessageAnalyserService(IAnimeAnalyserService animeAnalyserService, ITenorApiHelper tenorApiHelper,
                                            ILogger<MessageAnalyserService> logger, IDiscordService discordService)
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

    public async Task CheckMessageForAnimeAsync(SocketMessage message)
    {
        var user = message.Author as SocketGuildUser;
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

    private async Task<bool> MessageContainsAnimeAsync(SocketMessage message)
    {
        if (!string.IsNullOrEmpty(message.Content))
        {
            var messageMediaModel = MessageContainsMedia(message.Content);
            if (messageMediaModel.ContainsMedia)
            {
                double animeScore;
                if (!string.IsNullOrEmpty(messageMediaModel.EmojiId))
                {
                    var emojiUrl = $"https://cdn.discordapp.com/emojis/{messageMediaModel.EmojiId}.png";
                    animeScore = await GetAnimeScoreAsync(emojiUrl);
                }
                else
                {
                    var youtubeUrlModel = MessageContainsYouTubeLink(message.Content);
                    string url;
                    if (youtubeUrlModel.ContainsMedia)
                    {
                        url = $"https://i3.ytimg.com/vi/{youtubeUrlModel.Url}/maxresdefault.jpg";
                    }
                    else
                    {
                        url = messageMediaModel.Url;
                        if (message.Content.Contains("tenor.com") && !_imageFormats.Any(x => message.Content.Contains(x)))
                            url = await tenorApiHelper.GetDirectTenorGifUrlAsync(url);
                    }

                    animeScore = await GetAnimeScoreAsync(url);
                }

                if (animeScore > BotConstants.AnimeScoreTolerance)
                    return true;
            }
        }

        if (message.Attachments.Count <= 0) return false;
        {
            foreach (var attachment in message.Attachments)
            {
                var animeScore = await GetAnimeScoreAsync(attachment.ProxyUrl);

                if (!(animeScore > BotConstants.AnimeScoreTolerance)) continue;

                return true;
            }
        }

        return false;
    }

    private async Task<double> GetAnimeScoreAsync(string url)
    {
        var encodedUrl = Base64UrlEncoder.Encode(url);
        return await animeAnalyserService.GetAnimeScoreAsync(encodedUrl);
    }

    private static MessageMediaModel MessageContainsMedia(string messageContent)
    {
        foreach (var word in messageContent.Split())
        {
            var urlMatch = UrlRegex().Match(word);
            var emojiMatch = DiscordEmojiRegex().Match(word);

            if (!urlMatch.Success && !emojiMatch.Success) continue;

            return new MessageMediaModel
            {
                ContainsMedia = true,
                Url = urlMatch.Success ? urlMatch.Value : string.Empty,
                EmojiId = emojiMatch.Success ? emojiMatch.Groups.Values.ToArray()[1].ToString() : string.Empty
            };
        }

        return new MessageMediaModel
        {
            ContainsMedia = false
        };
    }

    private static MessageMediaModel MessageContainsYouTubeLink(string messageContent)
    {
        var youtubeUrlMatch = YouTubeUrlRegex().Match(messageContent);

        return new MessageMediaModel
        {
            ContainsMedia = youtubeUrlMatch.Success,
            Url = youtubeUrlMatch.Success ? youtubeUrlMatch.Groups.Values.ToList()[6].ToString() : string.Empty
        };
    }

    [GeneratedRegex("^https?:\\/\\/(?:www\\\\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b(?:[-a-zA-Z0-9()@:%_\\+.~#?&\\/=]*)$")]
    private static partial Regex UrlRegex();

    [GeneratedRegex("<:[a-zA-Z0-9]+:([0-9]+)>")]
    private static partial Regex DiscordEmojiRegex();

    [GeneratedRegex("^((?:https?:)?\\/\\/)?((?:www|m)\\.)?((?:youtube(-nocookie)?\\.com|youtu.be))(\\/(?:[\\w\\-]+\\?v=|embed\\/|v\\/)?)([\\w\\-]+)(\\S+)?$")]
    private static partial Regex YouTubeUrlRegex();
}