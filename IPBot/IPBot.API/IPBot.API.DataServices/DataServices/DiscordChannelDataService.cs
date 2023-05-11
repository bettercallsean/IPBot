using IPBot.API.DataServices.Data;
using IPBot.API.DataServices.Interfaces.DataServices;
using IPBot.API.DataServices.Models;
using Microsoft.EntityFrameworkCore;

namespace IPBot.API.DataServices.DataServices;

public class DiscordChannelDataService : IDiscordChannelDataService
{
    private readonly IIPBotDataContext _ipBotDataContext;

    public DiscordChannelDataService(IIPBotDataContext ipBotDataContext)
    {
        _ipBotDataContext = ipBotDataContext;
    }

    public async Task<List<DiscordChannel>> GetAllAsync()
    {
        return await _ipBotDataContext.DiscordChannels
            .AsNoTracking()
            .ToListAsync();
    }
}