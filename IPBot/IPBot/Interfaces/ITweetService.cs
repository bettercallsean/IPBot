namespace IPBot.Interfaces;

public interface ITweetService
{
    Task<string> GetDirectTweetImageLinkAsync(string twitterUrl);
    string GetFixUpXLink(string messageContent);
    bool ContentContainsTweetLink(string content);
}
