using System.Text.RegularExpressions;
using IPBot.Helpers;
using IPBot.Models;

namespace IPBot.Services;

internal class MessageAnalyserService
{
    private static readonly AnimeAnalyser.AnimeAnalyser AnimeAnalyser = new();
    private static readonly List<string> ResponseList = new()
    {
        "https://c.tenor.com/xwvZutw8Z7AAAAAC/tenor.gif",
        "https://64.media.tumblr.com/c045b0be831f9a3eaff6ef009d182f03/tumblr_mh958xh2jX1qa8a12o1_500.gif",
        "https://i2.wp.com/www.nerdsandbeyond.com/wp-content/uploads/2020/08/SmoggyHilariousBaiji-size_restricted.gif?resize=540%2C304",
        "https://c.tenor.com/NDe_7Jj8RaEAAAAd/tenor.gif",
        "https://tenor.com/view/unfunny-meme-citation-moa-a-citation-warning-issued-no-penalty-gif-16450177",
        "https://64.media.tumblr.com/2714da2b6d92e94bc21654b2d6e2a239/tumblr_mq3rkbHSDG1szkv9io1_500.gif",
        "https://gfycat.com/unhappyfatherlygoa",
        "https://c.tenor.com/JsV3NsOZPF4AAAAd/tenor.gif",
        "https://media.discordapp.net/attachments/140558319340355584/976269413047226378/Untitled.gif",
        "https://i.imgur.com/R6qrD.gif",
        "https://64.media.tumblr.com/tumblr_lv2kv1n4OU1r4zr2vo2_r4_500.gif"
    };
    
    public static async Task CheckMessageForAnimeAsync(SocketMessage message)
    {
        var channelNames = DebugHelper.IsDebug() ? new List<string> { "anti-anime-test" } : new List<string> { "anti-anime-test", "the-gospel" };

        if (channelNames.Contains(message.Channel.Name) && !message.Author.IsBot)
        {
            if (await MessageContainsAnimeAsync(message))
            {
                await message.DeleteAsync();
                await message.Channel.SendMessageAsync(ResponseList.OrderBy(_ => Guid.NewGuid()).Take(1).First());
            }
        }
    }

    private static async Task<bool> MessageContainsAnimeAsync(SocketMessage message)
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
                        url = message.Content + ".gif";
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

    private static async Task<double> GetAnimeScoreAsync(string url)
    {
        return await AnimeAnalyser.GetAnimeScoreAsync(url);
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