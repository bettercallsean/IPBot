using IPBot.Common.Dtos;
using IPBot.Common.Services;
using Microsoft.AspNetCore.Authorization;

namespace IPBot.API.Controllers;

[Authorize]
public class DiscordController(IDiscordService discordService) : MainController
{
    [HttpGet]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, NoStore = false)]
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
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, NoStore = false)]
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

    [HttpGet]
    [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any, NoStore = false)]
    public async Task<ActionResult<FlaggedUserDto>> GetFlaggedUserAsync(ulong userId)
    {
        try
        {
            return Ok(await discordService.GetFlaggedUserAsync(userId));
        }
        catch (Exception ex)
        {
            return Problem("500", ex.Message);
        }
    }

    [HttpGet]
    [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any, NoStore = false)]
    public async Task<ActionResult<bool>> UpdateUserFlaggedCountAsync(ulong userId)
    {
        try
        {
            return Ok(await discordService.UpdateUserFlaggedCountAsync(userId));
        }
        catch (Exception ex)
        {
            return Problem("500", ex.Message);
        }
    }

    [HttpPost]
    [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any, NoStore = false)]
    public async Task<ActionResult<bool>> CreateFlaggedUserAsync(FlaggedUserDto dto)
    {
        try
        {
            return Ok(await discordService.CreateFlaggedUserAsync(dto));
        }
        catch (Exception ex)
        {
            return Problem("500", ex.Message);
        }
    }
}