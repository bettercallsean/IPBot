﻿using IPBot.Common.Dtos;
using IPBot.Common.Services;
using Microsoft.AspNetCore.Authorization;

namespace IPBot.API.Controllers;

public class UserController : MainController
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<bool>> RegisterAsync(UserDto dto)
    {
        var result = await _userService.RegisterUserAsync(dto);
        return result ? Ok() : Problem();
    }

    [HttpPost]
    public async Task<ActionResult<string>> LoginAsync(UserDto dto)
    {
        var token = await _userService.LoginUserAsync(dto);

        return string.IsNullOrEmpty(token) ? BadRequest("Invalid credentials") : Ok(token);
    }
}
