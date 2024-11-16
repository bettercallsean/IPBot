using IPBot.Common.Services;
using Microsoft.AspNetCore.Authorization;

namespace IPBot.API.Controllers;

[Authorize]
public class DiscordController(IDiscordService discordService) : MainController
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

    [HttpGet]
    public async Task<ActionResult<bool>> ChannelIsBeingAnalysedForAnimeAsync(ulong guildId, ulong channelId)
    {
        try
        {
            return Ok(await discordService.ChannelIsBeingAnalysedForAnimeAsync(guildId, channelId));
        }
        catch (Exception ex)
        {
            return Problem("500", ex.Message);
        }
    }
}