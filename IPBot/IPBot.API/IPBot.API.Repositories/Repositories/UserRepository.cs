using IPBot.API.Repositories.Data;
using IPBot.API.Repositories.Interfaces;
using IPBot.API.Repositories.Models;

namespace IPBot.API.Repositories.Repositories;
public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(IIPBotDataContext ipBotDataContext) : base(ipBotDataContext) { }
}
