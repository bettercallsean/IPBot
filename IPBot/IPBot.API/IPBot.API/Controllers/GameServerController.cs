using IPBot.API.Shared.Services;
using IPBot.DTOs.Dtos;
using IPBot.Infrastructure.Models;

namespace IPBot.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameServerController : ControllerBase
{
    private readonly IGameService _gameService;

    public GameServerController(IGameService gameService)
    {
        _gameService = gameService;
    }

    [HttpGet("GetMinecraftServerStatus/{portNumber:int}")]
    public async Task<ActionResult<ServerInfo>> GetMinecraftServerStatusAsync(int portNumber)
    {
        try
        {
            return Ok(await _gameService.GetMinecraftServerStatusAsync(portNumber));
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
            return Ok(await _gameService.GetSteamServerStatusAsync(portNumber));
        }
        catch (Exception ex)
        {
            return Problem("500", ex.Message);
        }
    }

    [HttpGet("GetActiveServers/{gameName}")]
    public async Task<ActionResult<List<GameServerDto>>> GetActiveServersAsync(string gameName)
    {
        try
        {
            return Ok(await _gameService.GetActiveServersAsync(gameName));
        }
        catch (Exception ex)
        {
            return Problem("500", ex.Message);
        }
    }
}