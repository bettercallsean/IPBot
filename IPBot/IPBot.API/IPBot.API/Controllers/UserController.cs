using IPBot.DataServices.Dtos;
using IPBot.DataServices.Interfaces.Services;
using IPBot.DataServices.Models;

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

    [HttpPost("registerUser")]
    public async Task<ActionResult<User>> RegisterUserAsync(UserDto dto)
    {
        var user = await _userService.RegisterUserAsync(dto);
        return Ok(user);
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> LoginAsync(UserDto dto)
    {
        var token = await _userService.LoginUserAsync(dto);

        return string.IsNullOrEmpty(token) ? BadRequest("Invalid credentials") : Ok(token);
    }
}
