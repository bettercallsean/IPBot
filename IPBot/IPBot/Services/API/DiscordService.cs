using IPBot.Common.Dtos;
using IPBot.Common.Services;
using IPBot.Configuration;
using RestSharp;

namespace IPBot.Services.API;

public class DiscordService(IRestClient client, BotConfiguration botConfiguration) : ServiceBase(client, botConfiguration.APILogin), IDiscordService
{
    private const string BaseUri = "/Discord";

    public async Task<List<DiscordChannelDto>> GetActiveDiscordChannelsAsync()
    {
        return await GetAsync<List<DiscordChannelDto>>($"{BaseUri}/channels/active");
    }

    public async Task<bool> ChannelIsBeingAnalysedForAnimeAsync(ulong guildId, ulong channelId)
    {
        return await GetAsync<bool>($"{BaseUri}/guilds/{guildId}/channels/{channelId}/analyse-for-anime");
    }

    public async Task<FlaggedUserDto> GetFlaggedUserAsync(ulong userId)
    {
        return await GetAsync<FlaggedUserDto>($"{BaseUri}/users/flagged/{userId}");
    }

    public async Task<bool> IncrementUserFlaggedCountAsync(ulong userId)
    {
        return await GetAsync<bool>($"{BaseUri}/users/flagged/{userId}/increment");
    }

    public async Task<bool> CreateFlaggedUserAsync(FlaggedUserDto dto)
    {
        return await PostAsync<bool>($"{BaseUri}/users/flagged", dto);
    }

    public async Task<List<FlaggedUserDto>> GetFlaggedUsersAsync()
    {
        return await GetAsync<List<FlaggedUserDto>>($"{BaseUri}/users/flagged");
    }

    public async Task<bool> DeleteFlaggedUserAsync(ulong userId)
    {
        return await DeleteAsync<bool>($"{BaseUri}/users/flagged/{userId}");
    }

    public async Task<bool> GuildIsBeingCheckedForTwitterLinksAsync(ulong guildId)
    {
        return await GetAsync<bool>($"{BaseUri}/guilds/{guildId}/twitter-links");
    }

    public async Task<bool> ToggleTwitterLinkScanningAsync(ulong guildId)
    {
        return await PatchAsync<bool>($"{BaseUri}/guilds/{guildId}/twitter-links");
    }
}