namespace IPBot.Interfaces.Services;

public interface ITweetAnalyserService
{
    Task<List<string>> GetTweetVideoLinksAsync(TweetDetails tweetDetails);
    bool ContentContainsTweetLink(string content);
    TweetDetails GetTweetDetails(string tweetLink);
    Task CheckForTwitterLinksAsync(SocketMessage message);
}
