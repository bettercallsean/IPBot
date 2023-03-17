using IPBot.DataServices.Dtos;
using IPBot.DataServices.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;

namespace IPBot.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [Authorize]
    [HttpPost("RegisterUser")]
    public async Task<ActionResult<bool>> RegisterUserAsync(UserDto dto)
    {
        await _userService.RegisterUserAsync(dto);
        return Ok(true);
    }

    [HttpPost("Login")]
    public async Task<ActionResult<string>> LoginAsync(UserDto dto)
    {
        var token = await _userService.LoginUserAsync(dto);

        return string.IsNullOrEmpty(token) ? BadRequest("Invalid credentials") : Ok(token);
    }
}
