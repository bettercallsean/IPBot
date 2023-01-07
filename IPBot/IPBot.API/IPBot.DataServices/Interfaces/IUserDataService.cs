using IPBot.DataServices.Models;

namespace IPBot.DataServices.Interfaces;

public interface IUserDataService
{
    Task<User> GetByIdAsync(int id);
}
