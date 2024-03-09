using IPBot.Common.Dtos;

namespace IPBot.Common.Services;

public interface IDiscordService
{
    Task<List<DiscordChannelDto>> GetInUseDiscordChannelsAsync();
}