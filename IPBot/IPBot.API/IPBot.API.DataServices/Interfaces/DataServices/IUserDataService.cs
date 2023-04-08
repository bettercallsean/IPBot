using IPBot.DataServices.Models;

namespace IPBot.DataServices.Interfaces.DataServices;

public interface IUserDataService
{
    Task<User> GetByIdAsync(int id);
    Task<bool> CreateAsync(User user);
    Task<User> GetByUsernameAsync(string username);
}
