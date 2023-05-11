using IPBot.Shared.Dtos;

namespace IPBot.Shared.Services;

public interface IDiscordService
{
    Task<List<DiscordChannelDto>> GetInUseDiscordChannelsAsync();
}