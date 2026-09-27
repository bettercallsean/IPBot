using IPBot.Common.Dtos;

namespace IPBot.Common.Services;

public interface IDiscordService
{
    Task<List<DiscordChannelDto>> GetActiveDiscordChannelsAsync();
    Task<DiscordChannelDto> GetDiscordChannelAsync(ulong guildId, ulong channelId);
    Task<FlaggedUserDto> GetFlaggedUserAsync(ulong userId);
    Task<bool> IncrementUserFlaggedCountAsync(ulong userId);
    Task<bool> CreateFlaggedUserAsync(FlaggedUserDto dto);
    Task<List<FlaggedUserDto>> GetFlaggedUsersAsync();
    Task<bool> DeleteFlaggedUserAsync(ulong userId);
    Task<DiscordGuildDto> GetDiscordGuildAsync(ulong guildId);
    Task<bool> UpdateDiscordGuild(ulong guildId, DiscordGuildDto guildDto);
}