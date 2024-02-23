using IPBot.API.Domain.Data;
using IPBot.API.Domain.Entities;
using IPBot.API.Domain.Interfaces;

namespace IPBot.API.Domain.Repositories;

public class GameRepository(IIPBotDataContext ipBotDataContext) : BaseRepository<Game>(ipBotDataContext), IGameRepository
{
}
