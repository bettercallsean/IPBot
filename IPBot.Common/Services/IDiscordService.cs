using IPBot.Common.Dtos;

namespace IPBot.Common.Services;

public interface IDiscordService
{
    Task<List<DiscordChannelDto>> GetInUseDiscordChannelsAsync();
    Task<bool> ChannelIsBeingAnalysedForAnimeAsync(ulong guildId, ulong channelId);
    Task<bool> UserIsFlaggedAsync(ulong userId);
    Task<bool> UpdateUserFlaggedCountAsync(ulong userId);
}