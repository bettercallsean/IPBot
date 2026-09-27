using IPBot.Common.Dtos;
using IPBot.Common.Services;
using RestSharp;

namespace IPBot.Services.API;

internal sealed class DiscordService(IRestClient client) : RestService(client), IDiscordService
{
    private const string BaseUri = "/Discord";

    public async Task<List<DiscordChannelDto>> GetActiveDiscordChannelsAsync()
    {
        return await GetAsync<List<DiscordChannelDto>>($"{BaseUri}/channels/active");
    }

    public async Task<DiscordChannelDto> GetDiscordChannelAsync(ulong guildId, ulong channelId)
    {
        return await GetAsync<DiscordChannelDto>($"{BaseUri}/guilds/{guildId}/channels/{channelId}");
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

    public async Task<DiscordGuildDto> GetDiscordGuildAsync(ulong guildId)
    {
        return await GetAsync<DiscordGuildDto>($"{BaseUri}/guilds/{guildId}");
    }

    public async Task<bool> UpdateDiscordGuild(ulong guildId, DiscordGuildDto guildDto)
    {
        return await PutAsync<bool>($"{BaseUri}/guilds/{guildId}", guildDto);
    }
}