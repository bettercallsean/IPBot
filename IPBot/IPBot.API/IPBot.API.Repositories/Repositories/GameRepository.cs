using IPBot.API.Repositories.Data;
using IPBot.API.Repositories.Interfaces;
using IPBot.API.Repositories.Models;

namespace IPBot.API.Repositories.Repositories;

public class GameRepository : BaseRepository<Game>, IGameRepository
{
    public GameRepository(IIPBotDataContext ipBotDataContext) : base(ipBotDataContext) { }
}
