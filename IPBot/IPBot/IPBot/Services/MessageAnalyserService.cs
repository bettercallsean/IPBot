using System.Text.RegularExpressions;
using IPBot.Helpers;
using IPBot.Interfaces;
using IPBot.Shared.Services;

namespace IPBot.Services;

public partial class MessageAnalyserService
{
    private readonly List<string> _responseList = Resources.Resources.ResponseGifs.Split(Environment.NewLine).ToList();
    private readonly IAnimeAnalyserService _animeAnalyserService;
    private readonly ITenorApiHelper _tenorApiHelper;

    private readonly List<string> _imageFormats = new()
    {
        ".png",
        ".jpeg",
        ".jpg",
        ".tiff",
        ".mp4",
        ".gif"
    };

    public MessageAnalyserService(IAnimeAnalyserService animeAnalyserService, ITenorApiHelper tenorApiHelper)
    {
        _animeAnalyserService = animeAnalyserService;
        _tenorApiHelper = tenorApiHelper;
    }

    public async Task CheckMessageForAnimeAsync(SocketMessage message)
    {
        var channelNames = DebugHelper.IsDebug()
            ? new List<string> { "anti-anime-test" }
            : new List<string> { "anti-anime-test", "the-gospel" };

        if (channelNames.Contains(message.Channel.Name) && !message.Author.IsBot)
        {
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
                    if (youtubeUrlModel.ContainsMedia)
                    {
                        var url = $"https://i3.ytimg.com/vi/{youtubeUrlModel.Url}/maxresdefault.jpg";

                        animeScore = await GetAnimeScoreAsync(url);
                    }
                    else
                    {
                        var url = messageMediaModel.Url;
                        if (message.Content.Contains("tenor.com") && !_imageFormats.Any(x => message.Content.Contains(x)))
                        {
                            url = await _tenorApiHelper.GetDirectTenorGifUrlAsync(url);
                        }

                        animeScore = await GetAnimeScoreAsync(url);
                    }
                }

                if (animeScore > BotConstants.AnimeScoreTolerance)
                {
                    return true;
                }
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
        return await _animeAnalyserService.GetAnimeScoreAsync(url);
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