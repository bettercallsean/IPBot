using IPBot.API.Domain.Data;
using IPBot.API.Domain.Entities;
using IPBot.API.Domain.Interfaces;

namespace IPBot.API.Domain.Repositories;

public class GameServerRepository : BaseRepository<GameServer>, IGameServerRepository
{
    public GameServerRepository(IIPBotDataContext ipBotDataContext) : base(ipBotDataContext) { }
}
