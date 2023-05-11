using IPBot.API.Repositories.Data;
using IPBot.API.Repositories.Interfaces.Repositories;
using IPBot.API.Repositories.Models;

namespace IPBot.API.Repositories.Repositories;

public class DiscordChannelRepository : BaseRepository<DiscordChannel>, IDiscordChannelDataService
{
    public DiscordChannelRepository(IIPBotDataContext ipBotDataContext) : base(ipBotDataContext) { }
}