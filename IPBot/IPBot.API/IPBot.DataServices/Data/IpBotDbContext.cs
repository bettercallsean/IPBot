using IPBot.DataServices.Models;
using Microsoft.EntityFrameworkCore;

namespace IPBot.DataServices.Data;
public class IpBotDbContext : DbContext, IIpBotDataContext
{
    public IpBotDbContext(DbContextOptions<IpBotDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    public Task<int> SaveChangesAsync() => base.SaveChangesAsync();
}
