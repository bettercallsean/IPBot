using IPBot.Common.Services;
using Microsoft.AspNetCore.Authorization;

namespace IPBot.API.Controllers;

[Authorize]
public class IPController(IIPService ipService) : MainController
{
    private readonly IIPService _ipService = ipService;

    [HttpGet("current-domain")]
    public async Task<ActionResult<string>> GetCurrentServerDomainAsync()
    {
        return Ok(await _ipService.GetCurrentServerDomainAsync());
    }

    [HttpGet("local")]
    public async Task<ActionResult<string>> GetLocalIPAsync()
    {
        return Ok(await _ipService.GetLocalIPAsync());
    }

    [HttpGet("server")]
    public async Task<ActionResult<string>> GetServerIPAsync()
    {
        return Ok(await _ipService.GetServerIPAsync());
    }

    [HttpPatch("server")]
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