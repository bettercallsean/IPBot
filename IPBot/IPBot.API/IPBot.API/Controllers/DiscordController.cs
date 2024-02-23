using IPBot.Shared.Services;
using Microsoft.AspNetCore.Authorization;

namespace IPBot.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]/[action]")]
public class DiscordController(IDiscordService discordService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<string>> GetInUseDiscordChannelsAsync()
    {
        try
        {
            return Ok(await discordService.GetInUseDiscordChannelsAsync());
        }
        catch (Exception ex)
        {
            return Problem("500", ex.Message);
        }
    }
}