using IPBot.Shared.Dtos;
using IPBot.Shared.Services;
using Microsoft.AspNetCore.Authorization;

namespace IPBot.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]/[action]")]
public class GameServerController : ControllerBase
{
    private readonly IGameService _gameService;

    public GameServerController(IGameService gameService)
    {
        _gameService = gameService;
    }

    [HttpGet("{portNumber:int}")]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, NoStore = false)]
    public async Task<ActionResult<ServerInfoDto>> GetMinecraftServerStatusAsync(int portNumber)
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

    [HttpGet("{portNumber:int}")]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, NoStore = false)]
    public async Task<ActionResult<ServerInfoDto>> GetSteamServerStatusAsync(int portNumber)
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

    [HttpGet("{gameName}")]
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
    
    [HttpPost]
    public async Task<ActionResult<bool>> UpdateGameServerInformationAsync(GameServerDto dto)
    {
        try
        {
            return Ok(await _gameService.UpdateGameServerInformationAsync(dto));
        }
        catch (Exception ex)
        {
            return Problem("500", ex.Message);
        }
    }
}