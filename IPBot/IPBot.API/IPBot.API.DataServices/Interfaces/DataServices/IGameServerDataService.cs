using IPBot.API.DataServices.Models;

namespace IPBot.API.DataServices.Interfaces.DataServices;

public interface IGameServerDataService
{
    Task<GameServer> GetByIdAsync(int id);
    Task<bool> UpdateAsync(GameServer model);
    Task<GameServer> GetByPortAsync(int port);
}

