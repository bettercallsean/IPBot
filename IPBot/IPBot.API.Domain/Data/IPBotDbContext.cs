using IPBot.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IPBot.API.Domain.Data;
public class IPBotDbContext(DbContextOptions<IPBotDbContext> options) : DbContext(options), IIPBotDataContext
{
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Game> Games { get; set; }
    public virtual DbSet<GameServer> GameServers { get; set; }
    public virtual DbSet<Entities.Domain> Domains { get; set; }
    public virtual DbSet<DiscordChannel> DiscordChannels { get; set; }
    public virtual DbSet<DiscordGuild> DiscordGuilds { get; set; }
    public virtual DbSet<FlaggedUser> FlaggedUsers { get; set; }
    public new DbSet<T> Set<T>() where T : class => base.Set<T>();

    public Task<int> SaveChangesAsync() => base.SaveChangesAsync();
}
