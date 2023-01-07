using IPBot.DataServices.Models;
using Microsoft.EntityFrameworkCore;

namespace IPBot.DataServices.Data;
public interface IIpBotDataContext
{
    DbSet<User> Users { get; set; }
    Task<int> SaveChangesAsync();
}
