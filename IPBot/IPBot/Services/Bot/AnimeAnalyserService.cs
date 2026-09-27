using Discord;
using IPBot.Common.Services;
using IPBot.Constants;
using IPBot.Interfaces.Services;
using Microsoft.IdentityModel.Tokens;

namespace IPBot.Services.Bot;

internal class AnimeAnalyserService(IDiscordService discordService, ILogger<AnimeAnalyserService> logger, IImageAnalyserService imageAnalyserService, IMessageMediaAnalyserService messageAnalyserService) : IAnimeAnalyserService
{
    private readonly List<string> _responseList = [.. Resources.Resources.ResponseGifs.Split(Environment.NewLine)];
    private readonly IDiscordService _discordService = discordService;
    private readonly ILogger<AnimeAnalyserService> _logger = logger;
    private readonly IImageAnalyserService _imageAnalyserService = imageAnalyserService;
    private readonly IMessageMediaAnalyserService _messageAnalyserService = messageAnalyserService;

    public async Task CheckMessageForAnimeAsync(SocketMessage message)
    {
        var user = message.Author as IGuildUser;
        var discordChannel = await _discordService.GetDiscordChannelAsync(user.Guild.Id, message.Channel.Id);

        if (!discordChannel.AnalyseForAnime) return;

        if (await MessageContainsAnimeAsync(message))
        {
            await message.DeleteAsync();
            await message.Channel.SendMessageAsync(_responseList[Random.Shared.Next(_responseList.Count)]);
        }
    }

    private async Task<bool> MessageContainsAnimeAsync(SocketMessage message)
    {
        var contentUrls = await _messageAnalyserService.GetContentUrlsAsync(message);

        if (contentUrls.Count == 0) return false;

        var user = message.Author as IGuildUser;
        _logger.LogInformation("Checking message from {User} in {GuildName}:{ChannelName} for anime", user.Username, user.Guild.Name, message.Channel.Name);

        foreach (var url in contentUrls)
        {
            var animeScore = await GetAnimeScoreAsync(url);

            _logger.LogInformation("Anime score: {Score}", animeScore);

            if (animeScore < BotConstants.AnimeScoreTolerance) continue;

            return true;
        }

        return false;
    }

    private async Task<double> GetAnimeScoreAsync(string url)
    {
        var encodedUrl = Base64UrlEncoder.Encode(url);
        return await _imageAnalyserService.GetAnimeScoreAsync(encodedUrl);
    }
}
