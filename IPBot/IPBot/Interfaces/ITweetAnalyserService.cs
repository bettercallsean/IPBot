namespace IPBot.Interfaces;

public interface ITweetAnalyserService
{
    Task<List<string>> GetTweetVideoLinksAsync(TweetDetails tweetDetails);
    bool ContentContainsTweetLink(string content);
    TweetDetails GetTweetDetails(string tweetLink);
    string GetFixUpXLink(TweetDetails tweetDetails);
    Task CheckForTwitterLinksAsync(SocketMessage message);
}
