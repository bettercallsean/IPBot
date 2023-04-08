using IPBot.API.Shared.Dtos;

namespace IPBot.API.Shared.Services;

public interface IUserService
{
    Task<bool> RegisterUserAsync(UserDto dto);
    Task<string> LoginUserAsync(UserDto dto);
}
