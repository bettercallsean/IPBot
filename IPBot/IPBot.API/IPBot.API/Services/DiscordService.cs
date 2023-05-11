using AutoMapper;
using IPBot.API.DataServices.Interfaces.DataServices;
using IPBot.Shared.Dtos;
using IPBot.Shared.Services;

namespace IPBot.API.Services;

public class DiscordService : IDiscordService
{
    private readonly IMapper _mapper;
    private readonly IDiscordChannelDataService _discordChannelDataService;

    public DiscordService(IMapper mapper, IDiscordChannelDataService discordChannelDataService)
    {
        _mapper = mapper;
        _discordChannelDataService = discordChannelDataService;
    }

    public async Task<List<DiscordChannelDto>> GetInUseDiscordChannelsAsync()
    {
        var channels = await _discordChannelDataService.GetAllAsync();

        return _mapper.Map<List<DiscordChannelDto>>(channels.Where(x => x.InUse));
    }
}