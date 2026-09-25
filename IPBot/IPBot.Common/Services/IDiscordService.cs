using IPBot.Common.Dtos;

namespace IPBot.Common.Services;

public interface IDiscordService
{
    Task<List<DiscordChannelDto>> GetActiveDiscordChannelsAsync();
    Task<bool> ChannelIsBeingAnalysedForAnimeAsync(ulong guildId, ulong channelId);
    Task<FlaggedUserDto> GetFlaggedUserAsync(ulong userId);
    Task<bool> IncrementUserFlaggedCountAsync(ulong userId);
    Task<bool> CreateFlaggedUserAsync(FlaggedUserDto dto);
    Task<List<FlaggedUserDto>> GetFlaggedUsersAsync();
    Task<bool> DeleteFlaggedUserAsync(ulong userId);
    Task<bool> GuildIsBeingCheckedForTwitterLinksAsync(ulong guildId);
    Task<bool> ToggleTwitterLinkScanningAsync(ulong guildId);
}