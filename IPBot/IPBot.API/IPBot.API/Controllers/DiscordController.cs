using IPBot.Shared.Services;
using Microsoft.AspNetCore.Authorization;

namespace IPBot.API.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class DiscordController : ControllerBase
{
    private readonly IDiscordService _discordService;

    public DiscordController(IDiscordService discordService)
    {
        _discordService = discordService;
    }
    
    [HttpGet("GetInUseDiscordChannels")]
    public async Task<ActionResult<string>> GetInUseDiscordChannelsAsync()
    {
        try
        {
            return Ok(await _discordService.GetInUseDiscordChannelsAsync());
        }
        catch (Exception ex)
        {
            return Problem("500", ex.Message);
        }
    }
}