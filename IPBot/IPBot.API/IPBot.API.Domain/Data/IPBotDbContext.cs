﻿using IPBot.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IPBot.API.Domain.Data;
public class IPBotDbContext : DbContext, IIPBotDataContext
{
    public IPBotDbContext(DbContextOptions<IPBotDbContext> options) : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Game> Games { get; set; }
    public virtual DbSet<GameServer> GameServers { get; set; }
    public virtual DbSet<Entities.Domain> Domains { get; set; }
    public virtual DbSet<DiscordChannel> DiscordChannels { get; set; }
    public DbSet<T> Set<T>() where T : class => base.Set<T>();

    public Task<int> SaveChangesAsync() => base.SaveChangesAsync();
}