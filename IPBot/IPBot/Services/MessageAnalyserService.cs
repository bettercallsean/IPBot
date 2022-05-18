using System.Text.RegularExpressions;
using IPBot.Helpers;

namespace IPBot.Services;

public class MessageAnalyserService
{
    private static readonly List<string> ResponseList = Resources.Resources.ResponseGifs.Split(Environment.NewLine).ToList();
    private readonly AnimeAnalyser.AnimeAnalyser _animeAnalyser;
    private readonly TenorApiHelper _tenorApiHelper;

    public MessageAnalyserService(AnimeAnalyser.AnimeAnalyser animeAnalyser, TenorApiHelper tenorApiHelper)
    {
        _animeAnalyser = animeAnalyser;
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
                await message.Channel.SendMessageAsync(ResponseList.OrderBy(_ => Guid.NewGuid()).Take(1).First());
            }
        }
    }

    private async Task<bool> MessageContainsAnimeAsync(SocketMessage message)
    {
        if (!string.IsNullOrEmpty(message.Content))
        {
            var messageUrlModel = MessageContainsUrl(message.Content);
            if (messageUrlModel.Success)
            {
                double animeScore;

                var youtubeUrlModel = MessageContainsYouTubeLink(message.Content);
                if (youtubeUrlModel.Success)
                {
                    var url = $"https://i.ytimg.com/vi/{youtubeUrlModel.Url}/sddefault.jpg";

                    animeScore = await GetAnimeScoreAsync(url);
                }
                else
                {
                    var imageFormats = new List<string>
                    {
                        ".png",
                        ".jpeg",
                        ".jpg",
                        ".tiff",
                        ".mp4"
                    };

                    var url = messageUrlModel.Url;
                    if (!imageFormats.Any(x => message.Content.Contains(x)))
                    {
                        url = await _tenorApiHelper.GetDirectTenorGifUrlAsync(messageUrlModel.Url);
                    }

                    animeScore = await GetAnimeScoreAsync(url);
                }

                if (animeScore > 0)
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

                if (!(animeScore > 0)) continue;

                return true;
            }
        }

        return false;
    }

    private async Task<double> GetAnimeScoreAsync(string url)
    {
        return await _animeAnalyser.GetAnimeScoreAsync(url);
    }

    private static MessageUrlModel MessageContainsUrl(string messageContent)
    {
        foreach (var word in messageContent.Split())
        {
            var urlMatch = Regex.Match(word, @"^https?:\/\/(?:www\\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b(?:[-a-zA-Z0-9()@:%_\+.~#?&\/=]*)$");

            if (!urlMatch.Success) continue;

            return new MessageUrlModel
            {
                Success = true,
                Url = urlMatch.Success ? urlMatch.Value : string.Empty
            };
        }

        return new MessageUrlModel
        {
            Success = false
        };
    }

    private static MessageUrlModel MessageContainsYouTubeLink(string messageContent)
    {
        var youtubeUrlMatch = Regex.Match(messageContent, @"^((?:https?:)?\/\/)?((?:www|m)\.)?((?:youtube(-nocookie)?\.com|youtu.be))(\/(?:[\w\-]+\?v=|embed\/|v\/)?)([\w\-]+)(\S+)?$");

        return new MessageUrlModel
        {
            Success = youtubeUrlMatch.Success,
            Url = youtubeUrlMatch.Success ? youtubeUrlMatch.Groups.Values.ToList()[6].ToString() : string.Empty
        };
    }
}