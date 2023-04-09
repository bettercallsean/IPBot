using IPBot.Shared.Services;
using Microsoft.AspNetCore.Authorization;

namespace IPBot.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IPController : ControllerBase
{
    private readonly IIPService _ipService;

    public IPController(IIPService ipService)
    {
        _ipService = ipService;
    }
    
    [HttpGet("GetCurrentServerDomain")]
    public async Task<ActionResult<string>> GetCurrentDomain()
    {
        return Ok(await _ipService.GetCurrentServerDomainAsync());
    }

    [HttpGet("GetLocalIP")]
    public async Task<ActionResult<string>> GetLocalIPAsync()
    {
        return Ok(await _ipService.GetLocalIPAsync());
    }

    [HttpGet("GetServerIP")]
    public async Task<ActionResult<string>> GetServerIPAsync()
    {
        return Ok(await _ipService.GetServerIPAsync());
    }

    [Authorize]
    [HttpGet("UpdateServerIP")]
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