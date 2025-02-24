using IPBot.Common.Services;
using Microsoft.AspNetCore.Authorization;

namespace IPBot.API.Controllers;

[Authorize]
public class IPController(IIPService ipService) : MainController
{
    [HttpGet]
    public async Task<ActionResult<string>> GetCurrentServerDomainAsync()
    {
        return Ok(await ipService.GetCurrentServerDomainAsync());
    }

    [HttpGet]
    public async Task<ActionResult<string>> GetLocalIPAsync()
    {
        return Ok(await ipService.GetLocalIPAsync());
    }

    [HttpGet]
    public async Task<ActionResult<string>> GetServerIPAsync()
    {
        return Ok(await ipService.GetServerIPAsync());
    }
    
    [HttpGet("{ip}")]
    public async Task<ActionResult<bool>> UpdateServerIP(string ip)
    {
        try
        {
            return Ok(await ipService.UpdateServerIPAsync(ip));
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
}