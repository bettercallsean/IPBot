namespace IPBot.Interfaces;

public interface ITweetService
{
    Task<List<string>> GetTweetVideoLinksAsync(TweetDetails tweetDetails);
    bool ContentContainsTweetLink(string content);
    TweetDetails GetTweetDetails(string tweetLink);
    string GetFixUpXLink(TweetDetails tweetDetails);
}
