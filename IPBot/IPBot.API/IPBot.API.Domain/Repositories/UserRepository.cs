using IPBot.API.Domain.Data;
using IPBot.API.Domain.Entities;
using IPBot.API.Domain.Interfaces;

namespace IPBot.API.Domain.Repositories;
public class UserRepository(IIPBotDataContext ipBotDataContext) : BaseRepository<User>(ipBotDataContext), IUserRepository
{
}
