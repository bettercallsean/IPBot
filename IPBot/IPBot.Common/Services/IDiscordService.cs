using IPBot.Common.Dtos;

namespace IPBot.Common.Services;

public interface IDiscordService
{
    Task<List<DiscordChannelDto>> GetInUseDiscordChannelsAsync();
    Task<bool> ChannelIsBeingAnalysedForAnimeAsync(ulong guildId, ulong channelId);
    Task<FlaggedUserDto> GetFlaggedUserAsync(ulong userId);
    Task<bool> UpdateUserFlaggedCountAsync(ulong userId);
    Task<bool> CreateFlaggedUserAsync(FlaggedUserDto dto);
    Task<List<FlaggedUserDto>> GetFlaggedUsersAsync();
    Task<bool> DeleteFlaggedUserAsync(ulong userId);
}