﻿using IPBot.Shared.Services;
using Microsoft.AspNetCore.Authorization;

namespace IPBot.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]/[action]")]
public class IPController : ControllerBase
{
    private readonly IIPService _ipService;

    public IPController(IIPService ipService)
    {
        _ipService = ipService;
    }
    
    [HttpGet]
    public async Task<ActionResult<string>> GetCurrentServerDomainAsync()
    {
        return Ok(await _ipService.GetCurrentServerDomainAsync());
    }

    [HttpGet]
    public async Task<ActionResult<string>> GetLocalIPAsync()
    {
        return Ok(await _ipService.GetLocalIPAsync());
    }

    [HttpGet]
    public async Task<ActionResult<string>> GetServerIPAsync()
    {
        return Ok(await _ipService.GetServerIPAsync());
    }
    
    [HttpGet]
    public async Task<ActionResult<bool>> UpdateServerIP(string ip)
    {
        try
        {
            return Ok(await _ipService.UpdateServerIPAsync(ip));
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
}