using IPBot.API.Domain.Data;
using IPBot.API.Domain.Entities;
using IPBot.API.Domain.Interfaces;

namespace IPBot.API.Domain.Repositories;
public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(IIPBotDataContext ipBotDataContext) : base(ipBotDataContext) { }
}
