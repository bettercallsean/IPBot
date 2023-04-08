using IPBot.DataServices.Models;

namespace IPBot.DataServices.Interfaces.DataServices;

public interface IGameDataService
{
    Task<Game> GetByIdAsync(int id);
    Task<Game> GetByNameAsync(string name);
    Task<Game> GetByShortNameAsync(string shortName);
}

