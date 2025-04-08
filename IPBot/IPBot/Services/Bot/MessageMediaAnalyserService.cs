using IPBot.Helpers;
using IPBot.Interfaces;

namespace IPBot.Services.Bot;

public class MessageMediaAnalyserService : IMessageMediaAnalyserService
{
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

    private readonly ITenorApiHelper _tenorApiHelper;
    private readonly HttpClient _httpClient;

    public MessageMediaAnalyserService(ITenorApiHelper tenorApiHelper, HttpClient httpClient)
    {
        _tenorApiHelper = tenorApiHelper;
        _httpClient = httpClient;
    }

    public async Task<List<string>> GetContentUrlsAsync(SocketMessage message)
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

    private async Task<string> GetMediaUrlAsync(MessageMediaModel messageMediaModel, SocketMessage message)
    {
        if (!string.IsNullOrEmpty(messageMediaModel.EmojiId))
            return $"https://cdn.discordapp.com/emojis/{messageMediaModel.EmojiId}.png";

        var youtubeUrlModel = MessageContainsYouTubeLink(message.Content);
        if (youtubeUrlModel.ContainsMedia)
            return $"https://i3.ytimg.com/vi/{youtubeUrlModel.Url}/maxresdefault.jpg";

        var url = messageMediaModel.Url;

        if (message.Content.Contains("tenor.com") && !_imageFormats.Any(message.Content.Contains))
            return await _tenorApiHelper.GetDirectTenorGifUrlAsync(url);

        var result = await _httpClient.SendAsync(new(HttpMethod.Head, url));

        return _httpImageContentTypes.Contains(result.Content.Headers.ContentType?.MediaType) ? url : string.Empty;
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