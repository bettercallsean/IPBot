using IPBot.API.Repositories.Data;
using IPBot.API.Repositories.Interfaces.Repositories;
using IPBot.API.Repositories.Models;

namespace IPBot.API.Repositories.Repositories;
public class UserRepository : BaseRepository<User>, IUserDataService
{
    public UserRepository(IIPBotDataContext ipBotDataContext) : base(ipBotDataContext) { }
}
