using AutoMapper;
using IPBot.API.Repositories.Interfaces;
using IPBot.Shared.Dtos;
using IPBot.Shared.Services;

namespace IPBot.API.Services;

public class DiscordService : IDiscordService
{
    private readonly IMapper _mapper;
    private readonly IDiscordChannelRepository _discordChannelRepository;

    public DiscordService(IMapper mapper, IDiscordChannelRepository discordChannelRepository)
    {
        _mapper = mapper;
        _discordChannelRepository = discordChannelRepository;
    }

    public async Task<List<DiscordChannelDto>> GetInUseDiscordChannelsAsync()
    {
        var channels = await _discordChannelRepository.GetWhereAsync(x => x.InUse);

        return _mapper.Map<List<DiscordChannelDto>>(channels);
    }
}