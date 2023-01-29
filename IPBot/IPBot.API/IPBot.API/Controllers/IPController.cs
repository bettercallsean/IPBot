using Microsoft.AspNetCore.Authorization;

namespace IPBot.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IPController : ControllerBase
{
    [HttpGet("GetCurrentServerDomain")]
    public ActionResult<string> GetCurrentDomain()
    {
        return Ok(ServerDomainHelper.GetCurrentServerDomain());
    }

    [HttpGet("GetLocalIP")]
    public async Task<ActionResult<string>> GetLocalIPAsync()
    {
        return Ok(await IPHelper.GetLocalIPAsync());
    }

    [HttpGet("GetServerIP")]
    public async Task<ActionResult<string>> GetServerIPAsync()
    {
        return Ok(await IPHelper.GetSeverIPAsync());
    }

    [Authorize]
    [HttpGet("UpdateServerIP")]
    public ActionResult<bool> UpdateServerIP(string ip)
    {
        try
        {
            return Ok(IPHelper.UpdateServerIP(ip));
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
}