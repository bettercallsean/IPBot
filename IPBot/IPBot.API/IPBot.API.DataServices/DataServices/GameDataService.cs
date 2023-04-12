﻿using IPBot.API.DataServices.Data;
using IPBot.API.DataServices.Interfaces.DataServices;
using IPBot.API.DataServices.Models;
using Microsoft.EntityFrameworkCore;

namespace IPBot.API.DataServices.DataServices;

public class GameDataService : IGameDataService
{
    private readonly IIPBotDataContext _ipBotDataContext;

    public GameDataService(IIPBotDataContext ipBotDataContext)
	{
        _ipBotDataContext = ipBotDataContext;
    }

    public async Task<Game> GetByIdAsync(int id)
    {
        return await _ipBotDataContext.Games.FindAsync(id);
    }

    public async Task<Game> GetByNameAsync(string name)
    {
        return await _ipBotDataContext.Games
            .AsNoTracking()
            .Include(x => x.GameServers)
            .Where(x => x.Name == name)
            .FirstOrDefaultAsync();
    }

    public async Task<Game> GetByShortNameAsync(string shortName)
    {
        return await _ipBotDataContext.Games
            .AsNoTracking()
            .Include(x => x.GameServers)
            .Where(x => x.ShortName == shortName)
            .FirstOrDefaultAsync();
    }
}
