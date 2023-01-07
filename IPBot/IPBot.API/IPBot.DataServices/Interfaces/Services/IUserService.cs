using IPBot.DataServices.Dtos;
using IPBot.DataServices.Models;

namespace IPBot.DataServices.Interfaces.Services;

public interface IUserService
{
    Task<User> RegisterUserAsync(UserDto dto);
    Task<string> LoginUserAsync(UserDto dto);
}
