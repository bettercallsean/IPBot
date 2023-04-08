using IPBot.DataServices.Models;

namespace IPBot.DataServices.Interfaces.DataServices;

public interface IGameServerDataService
{
    Task<GameServer> GetByIdAsync(int id);
    Task<GameServer> GetByPortAsync(int port);
}

