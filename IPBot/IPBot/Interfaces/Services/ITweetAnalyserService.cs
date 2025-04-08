namespace IPBot.Interfaces.Services;

public interface ITweetAnalyserService
{
    Task<List<string>> GetTweetVideoLinksAsync(TweetDetails tweetDetails);
    bool ContentContainsTweetLink(string content);
    TweetDetails GetTweetDetails(string tweetLink);
    string GetFixUpXLink(TweetDetails tweetDetails);
    Task CheckForTwitterLinksAsync(SocketMessage message);
}
