using System.Text.RegularExpressions;
using Discord;
using IPBot.Helpers;
using IPBot.Interfaces;

namespace IPBot.Services.Bot;

public class TweetService : ITweetService
{
    private const string XUrl = "https://x.com";
    private const string FixUpXUrl = "https://fixupx.com";
    
    private readonly HttpClient _httpClient;

    public TweetService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<string> GetDirectTweetImageLinkAsync(string twitterUrl)
    {
        var extractedLink = GetFixUpXLink(twitterUrl);

        if (string.IsNullOrEmpty(extractedLink)) return string.Empty;
        
        var stringBuilder = new StringBuilder(extractedLink);
        stringBuilder.Append(".jpg");

        var response = await _httpClient.GetAsync(stringBuilder.ToString());
        var imageUrl = response.RequestMessage?.RequestUri?.ToString();

        return imageUrl.Contains("twimg.com") ? imageUrl : string.Empty;
    }

    public string GetFixUpXLink(string messageContent)
    {
        var tweetLink = RegexHelper.TwitterLinkRegex().Match(messageContent);
        
        return tweetLink.Success ? tweetLink.Value.Replace(XUrl, FixUpXUrl) : string.Empty;
    }
    
    public bool ContentContainsTweetLink(string content) => RegexHelper.TwitterLinkRegex().Match(content).Success;
}