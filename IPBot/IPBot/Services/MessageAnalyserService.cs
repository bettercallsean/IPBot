using System.Text.RegularExpressions;

namespace IPBot.Services;

internal class MessageAnalyserService
{
    private static readonly AnimeAnalyser.AnimeAnalyser AnimeAnalyser = new();
    
    public static async Task<bool> MessageContainsAnimeAsync(SocketMessage message)
    {
        if (message.Content != null)
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