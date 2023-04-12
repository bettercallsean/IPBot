using IPBot.API.DataServices.Data;
using IPBot.API.DataServices.Interfaces.DataServices;
using IPBot.API.DataServices.Models;
using Microsoft.EntityFrameworkCore;

namespace IPBot.API.DataServices.DataServices;
public class UserDataService : IUserDataService
{
    private readonly IIPBotDataContext _ipBotDataContext;

    public UserDataService(IIPBotDataContext ipBotDataContext)
    {
        _ipBotDataContext = ipBotDataContext;
    }

    public async Task<User> GetByIdAsync(int id)
    {
        return await _ipBotDataContext.Users.FindAsync(id);
    }

    public async Task<bool> CreateAsync(User user)
    {
        _ipBotDataContext.Users.Add(user);

        return await _ipBotDataContext.SaveChangesAsync() > 0;
    }

    public async Task<User> GetByUsernameAsync(string username)
    {
        return await _ipBotDataContext.Users.FirstOrDefaultAsync(x => x.Username == username);
    }
}
