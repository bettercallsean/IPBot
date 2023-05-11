using IPBot.API.Repositories.Data;
using IPBot.API.Repositories.Interfaces.Repositories;
using IPBot.API.Repositories.Models;

namespace IPBot.API.Repositories.Repositories;

public class GameRepository : BaseRepository<Game>, IGameDataService
{
    public GameRepository(IIPBotDataContext ipBotDataContext) : base(ipBotDataContext) { }
}
