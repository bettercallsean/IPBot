using IPBot.Infrastructure.Models;

namespace IPBot.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameServerController : ControllerBase, IGameServerService
{
    [HttpGet("GetMinecraftServerStatus/{portNumber:int}")]
    public async Task<ServerInfo> GetMinecraftServerStatusAsync(int portNumber)
    {
        return await ServerInfoHelper.GetServerInfoAsync(Constants.MinecraftServerCode, portNumber);
    }
    
    [HttpGet("GetSteamServerStatus/{portNumber:int}")]
    public async Task<ServerInfo> GetSteamServerStatusAsync(int portNumber)
    {
        return await ServerInfoHelper.GetServerInfoAsync(Constants.SteamServerCode, portNumber);
    }
}