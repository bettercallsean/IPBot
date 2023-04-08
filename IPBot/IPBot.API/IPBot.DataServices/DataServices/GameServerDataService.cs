using IPBot.DataServices.Data;
using IPBot.DataServices.Interfaces.DataServices;
using IPBot.DataServices.Models;
using Microsoft.EntityFrameworkCore;

namespace IPBot.DataServices.DataServices;

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

    public async Task<GameServer> GetByPortAsync(int port)
    {
        return await _ipBotDataContext.GameServers
            .AsNoTracking()
            .Where(x => x.Port == port)
            .FirstOrDefaultAsync();
    }
}
