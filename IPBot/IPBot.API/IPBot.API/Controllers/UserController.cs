using IPBot.Shared.Dtos;
using IPBot.Shared.Services;
using Microsoft.AspNetCore.Authorization;

namespace IPBot.API.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class UserController(IUserService userService) : ControllerBase
{
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<bool>> RegisterAsync(UserDto dto)
    {
        var result = await userService.RegisterUserAsync(dto);
        return result ? Ok() : Problem();
    }

    [HttpPost]
    public async Task<ActionResult<string>> LoginAsync(UserDto dto)
    {
        var token = await userService.LoginUserAsync(dto);

        return string.IsNullOrEmpty(token) ? BadRequest("Invalid credentials") : Ok(token);
    }
}
