using IPBot.Common.Dtos;

namespace IPBot.Common.Services;

public interface IUserService
{
    Task<bool> RegisterUserAsync(UserDto dto);
    Task<string> LoginUserAsync(UserDto dto);
}
