using IPBot.DataServices.Data;
using IPBot.DataServices.Interfaces;
using IPBot.DataServices.Models;

namespace IPBot.DataServices.DataServices;
public class UserDataService : IUserDataService
{
    private readonly IIpBotDataContext _ipBotDataContext;

    public UserDataService(IIpBotDataContext ipBotDataContext)
    {
        _ipBotDataContext = ipBotDataContext;
    }

    public async Task<User> GetByIdAsync(int id)
    {
        return await _ipBotDataContext.Users.FindAsync(id);
    }
}
