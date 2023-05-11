using IPBot.API.DataServices.Models;
using Microsoft.EntityFrameworkCore;

namespace IPBot.API.DataServices.Data;

public interface IIPBotDataContext
{
    DbSet<User> Users { get; set; }
    DbSet<Game> Games { get; set; }
    DbSet<GameServer> GameServers { get; set; }
    DbSet<Domain> Domains { get; set; }
    DbSet<DiscordChannel> DiscordChannels { get; set; }
    DbSet<T> Set<T>() where T : class;
    Task<int> SaveChangesAsync();
}
