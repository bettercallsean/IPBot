using IPBot.API.Repositories.Data;
using IPBot.API.Repositories.Interfaces.Repositories;
using IPBot.API.Repositories.Models;

namespace IPBot.API.Repositories.Repositories;

public class GameServerRepository : BaseRepository<GameServer>, IGameServerDataService
{
    public GameServerRepository(IIPBotDataContext ipBotDataContext) : base(ipBotDataContext) { }
}
