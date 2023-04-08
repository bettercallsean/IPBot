using IPBot.DataServices.Models;
using Microsoft.EntityFrameworkCore;

namespace IPBot.DataServices.Data;

public interface IIPBotDataContext
{
    DbSet<User> Users { get; set; }
    DbSet<Game> Games { get; set; }
    DbSet<GameServer> GameServers { get; set; }
    Task<int> SaveChangesAsync();
}
