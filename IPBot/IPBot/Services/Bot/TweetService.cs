using IPBot.Helpers;
using IPBot.Interfaces;
using IPBot.Models.FixUpXModels;

namespace IPBot.Services.Bot;

public class TweetService : ITweetService
{
    private readonly HttpClient _httpClient;

    public TweetService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<List<string>> GetTweetVideoLinksAsync(TweetDetails tweetDetails)
    {
        var tweet = await GetTweetAsync(tweetDetails);
        
        return tweet.Media.Videos.Count > 0 ? tweet.Media.Videos.Select(x => x.Variants.MaxBy(y => y.Bitrate).Url).ToList() : [];
    }
    
    public bool ContentContainsTweetLink(string content) => RegexHelper.TwitterLinkRegex().Match(content).Success;
    
    public TweetDetails GetTweetDetails(string tweetLink)
    {
        var tweetRegex = RegexHelper.TwitterLinkRegex().Match(tweetLink);

        return new TweetDetails(tweetRegex.Groups[2].Value, ulong.Parse(tweetRegex.Groups[4].Value));
    }

    public string GetFixUpXLink(TweetDetails tweetDetails) => $"https://fixupx.com/{tweetDetails.Username}/status/{tweetDetails.Id}";

    private async Task<Tweet> GetTweetAsync(TweetDetails tweetDetails)
    {
        const string UserAgent = "IPBot/1.0.0.0 Discord Bot";
        _httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);

        var fixUpX = await _httpClient.GetFromJsonAsync<Root>($"https://api.fxtwitter.com/{tweetDetails.Username}/status/{tweetDetails.Id}");

        return fixUpX.Tweet;
    }
}