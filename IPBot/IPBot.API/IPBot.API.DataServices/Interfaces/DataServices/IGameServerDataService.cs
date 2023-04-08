using IPBot.DataServices.Models;

namespace IPBot.DataServices.Interfaces.DataServices;

public interface IGameServerDataService
{
    Task<GameServer> GetByIdAsync(int id);
    Task<bool> UpdateAsync(GameServer model);
    Task<GameServer> GetByPortAsync(int port);
}

