using IPBot.API.Repositories.Data;
using IPBot.API.Repositories.Interfaces;
using IPBot.API.Repositories.Models;

namespace IPBot.API.Repositories.Repositories;

public class DiscordChannelRepository : BaseRepository<DiscordChannel>, IDiscordChannelRepository
{
    public DiscordChannelRepository(IIPBotDataContext ipBotDataContext) : base(ipBotDataContext) { }
}