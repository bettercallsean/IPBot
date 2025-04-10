using Discord;
using IPBot.Common.Services;
using IPBot.Helpers;
using IPBot.Interfaces.Services;
using IPBot.Models.FixUpXModels;

namespace IPBot.Services.Bot;

public class TweetAnalyserService : ITweetAnalyserService
{
    private readonly HttpClient _httpClient;
    private readonly IDiscordService _discordService;
    private readonly ILogger<TweetAnalyserService> _logger;

    public TweetAnalyserService(HttpClient httpClient, IDiscordService discordService, ILogger<TweetAnalyserService> logger)
    {
        _httpClient = httpClient;
        _discordService = discordService;
        _logger = logger;
    }

    public async Task CheckForTwitterLinksAsync(SocketMessage message)
    {
        var channel = message.Channel as SocketGuildChannel;
        var guildIsBeingCheckedForTwitterLinks =
            await _discordService.GuidIsBeingCheckedForTwitterLinksAsync(channel.Guild.Id);

        if (!guildIsBeingCheckedForTwitterLinks) return;

        _logger.LogInformation("Checking message from {User} in {GuildName}:{ChannelName} for twitter links", message.Author.Username, channel.Guild.Name, channel.Name);

        await ReplyWithFixUpXLinkAsync(message, channel);
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
        const string UserAgent = "IPBot/1.0 +https://github.com/bettercallsean/IPBot Discord Bot";
        _httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);

        var fixUpX = await _httpClient.GetFromJsonAsync<Root>($"https://api.fxtwitter.com/{tweetDetails.Username}/status/{tweetDetails.Id}");

        return fixUpX.Tweet;
    }

    private async Task ReplyWithFixUpXLinkAsync(SocketMessage message, SocketGuildChannel channel)
    {
        if (ContentContainsTweetLink(message.Content) && message is IUserMessage userMessage)
        {
            var tweetDetails = GetTweetDetails(userMessage.Content);
            var fixUpXLink = GetFixUpXLink(tweetDetails);

            _logger.LogInformation("Responding to {Username} in {GuildName}:{ChannelName} with fixed link {Url}",
                message.Author.Username, channel.Guild.Name, channel.Name, fixUpXLink);

            await userMessage.Channel.SendMessageAsync($"{tweetDetails.Username}]({fixUpXLink})");
        }
    }
}