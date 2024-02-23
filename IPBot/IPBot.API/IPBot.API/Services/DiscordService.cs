using AutoMapper;
using IPBot.API.Domain.Interfaces;
using IPBot.Shared.Dtos;
using IPBot.Shared.Services;

namespace IPBot.API.Services;

public class DiscordService(IMapper mapper, IDiscordChannelRepository discordChannelRepository) : IDiscordService
{
    public async Task<List<DiscordChannelDto>> GetInUseDiscordChannelsAsync()
    {
        var channels = await discordChannelRepository.GetAllWhereAsync(x => x.InUse);

        return mapper.Map<List<DiscordChannelDto>>(channels);
    }
}