using System.Text.RegularExpressions;
using IPBot.Helpers;

namespace IPBot.Services;

internal class MessageAnalyserService
{
    private static readonly AnimeAnalyser.AnimeAnalyser AnimeAnalyser = new();
    
    private static readonly List<string> ResponseList = new()
    {
        "https://c.tenor.com/xwvZutw8Z7AAAAAC/tenor.gif",
        "https://64.media.tumblr.com/c045b0be831f9a3eaff6ef009d182f03/tumblr_mh958xh2jX1qa8a12o1_500.gif",
        "https://i2.wp.com/www.nerdsandbeyond.com/wp-content/uploads/2020/08/SmoggyHilariousBaiji-size_restricted.gif?resize=540%2C304",
        "https://c.tenor.com/NDe_7Jj8RaEAAAAd/tenor.gif"
    };

    public static async Task CheckMessageForAnimeAsync(SocketMessage message)
    {
        var channelName = DebugHelper.IsDebug() ? "anti-anime-test" : "the-gospel";

        if (message.Channel.Name == channelName && !message.Author.IsBot)
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
            double animeScore;

            var youtubeRegex =
                new Regex(
                    @"^((?:https?:)?\/\/)?((?:www|m)\.)?((?:youtube(-nocookie)?\.com|youtu.be))(\/(?:[\w\-]+\?v=|embed\/|v\/)?)([\w\-]+)(\S+)?$");
            var isYouTubeUrl = youtubeRegex.Match(message.Content);
                
            if (isYouTubeUrl.Success)
            {
                var watchId = isYouTubeUrl.Groups.Values.ToArray()[6];
                var url = $"https://i.ytimg.com/vi/{watchId}/sddefault.jpg";
                    
                animeScore = await GetAnimeScoreAsync(url);
            }
            else
            {
                var imageFormats = new List<string>
                {
                    ".png",
                    ".jpeg",
                    ".jpg",
                    ".tiff"
                };
                
                var url = message.Content;
                if(!imageFormats.Contains(message.Content[new Range(message.Content.Length - 4, message.Content.Length)]) )
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
}