using IPBot.API.Repositories.Data;
using IPBot.API.Repositories.Interfaces;
using IPBot.API.Repositories.Models;

namespace IPBot.API.Repositories.Repositories;

public class GameServerRepository : BaseRepository<GameServer>, IGameServerRepository
{
    public GameServerRepository(IIPBotDataContext ipBotDataContext) : base(ipBotDataContext) { }
}
