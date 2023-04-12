using IPBot.API.DataServices.Models;
using Microsoft.EntityFrameworkCore;

namespace IPBot.API.DataServices.Data;
public class IPBotDbContext : DbContext, IIPBotDataContext
{
    public IPBotDbContext(DbContextOptions<IPBotDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<GameServer> GameServers { get; set; }
    public DbSet<Domain> Domains { get; set; }

    public Task<int> SaveChangesAsync() => base.SaveChangesAsync();
}
