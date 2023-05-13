using IPBot.API.Domain.Data;
using IPBot.API.Domain.Entities;
using IPBot.API.Domain.Interfaces;

namespace IPBot.API.Domain.Repositories;

public class DiscordChannelRepository : BaseRepository<DiscordChannel>, IDiscordChannelRepository
{
    public DiscordChannelRepository(IIPBotDataContext ipBotDataContext) : base(ipBotDataContext) { }
}