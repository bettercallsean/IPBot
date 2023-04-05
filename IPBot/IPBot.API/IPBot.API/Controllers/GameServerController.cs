using IPBot.Infrastructure.Models;

namespace IPBot.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameServerController : ControllerBase
{
    [HttpGet("GetMinecraftServerStatus/{portNumber:int}")]
    public async Task<ActionResult<ServerInfo>> GetMinecraftServerStatusAsync(int portNumber)
    {
        try
        {
            return Ok(await ServerStatusHelper.GetServerInfoAsync(Constants.MinecraftServerCode, portNumber));
        }
        catch (Exception ex)
        {
            return Problem("500", ex.Message);
        }
    }

    [HttpGet("GetSteamServerStatus/{portNumber:int}")]
    public async Task<ActionResult<ServerInfo>> GetSteamServerStatusAsync(int portNumber)
    {
        try
        {
            return Ok(await ServerStatusHelper.GetServerInfoAsync(Constants.SteamServerCode, portNumber));
        }
        catch (Exception ex)
        {
            return Problem("500", ex.Message);
        }
    }
}