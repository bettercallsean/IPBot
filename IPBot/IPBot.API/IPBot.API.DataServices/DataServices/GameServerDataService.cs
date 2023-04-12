using IPBot.API.DataServices.Data;
using IPBot.API.DataServices.Interfaces.DataServices;
using IPBot.API.DataServices.Models;
using Microsoft.EntityFrameworkCore;

namespace IPBot.API.DataServices.DataServices;

public class GameServerDataService : IGameServerDataService
{
    private readonly IIPBotDataContext _ipBotDataContext;

    public GameServerDataService(IIPBotDataContext ipBotDataContext)
	{
        _ipBotDataContext = ipBotDataContext;
    }

    public async Task<GameServer> GetByIdAsync(int id)
    {
        return await _ipBotDataContext.GameServers.FindAsync(id);
    }

    public async Task<bool> UpdateAsync(GameServer model)
    {
        _ipBotDataContext.GameServers.Attach(model).State = EntityState.Modified;

        return await _ipBotDataContext.SaveChangesAsync() > 0;
    }

    public async Task<GameServer> GetByPortAsync(int port)
    {
        return await _ipBotDataContext.GameServers
            .AsNoTracking()
            .Where(x => x.Port == port)
            .FirstOrDefaultAsync();
    }
}
