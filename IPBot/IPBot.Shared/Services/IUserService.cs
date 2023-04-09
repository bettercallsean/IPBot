using IPBot.Shared.Dtos;

namespace IPBot.Shared.Services;

public interface IUserService
{
    Task<bool> RegisterUserAsync(UserDto dto);
    Task<string> LoginUserAsync(UserDto dto);
}
