using IPBot.Common.Dtos;
using IPBot.Common.Services;
using IPBot.Configuration;
using RestSharp;

namespace IPBot.Services.API;

public class DiscordService(IRestClient client, BotConfiguration botConfiguration) : ServiceBase(client, botConfiguration.APILogin), IDiscordService
{
    private const string BaseUri = "/Discord";

    public async Task<List<DiscordChannelDto>> GetInUseDiscordChannelsAsync()
    {
        return await GetAsync<List<DiscordChannelDto>>($"{BaseUri}/GetInUseDiscordChannels");
    }

    public async Task<bool> ChannelIsBeingAnalysedForAnimeAsync(ulong guildId, ulong channelId)
    {
        return await GetAsync<bool>($"{BaseUri}/ChannelIsBeingAnalysedForAnime?guildId={guildId}&channelId={channelId}");
    }
}