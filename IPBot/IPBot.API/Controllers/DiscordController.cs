using IPBot.Common.Dtos;
using IPBot.Common.Services;
using Microsoft.AspNetCore.Authorization;

namespace IPBot.API.Controllers;

[Authorize]
public class DiscordController : MainController
{
    private readonly IDiscordService _discordService;

    public DiscordController(IDiscordService discordService)
    {
        _discordService = discordService;
    }

    [HttpGet]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, NoStore = false)]
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

    [HttpGet]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, NoStore = false)]
    public async Task<ActionResult<bool>> ChannelIsBeingAnalysedForAnimeAsync(ulong guildId, ulong channelId)
    {
        try
        {
            return Ok(await _discordService.ChannelIsBeingAnalysedForAnimeAsync(guildId, channelId));
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
            return Ok(await _discordService.GetFlaggedUserAsync(userId));
        }
        catch (Exception ex)
        {
            return Problem("500", ex.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<bool>> UpdateUserFlaggedCountAsync(ulong userId)
    {
        try
        {
            return Ok(await _discordService.UpdateUserFlaggedCountAsync(userId));
        }
        catch (Exception ex)
        {
            return Problem("500", ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<bool>> CreateFlaggedUserAsync(FlaggedUserDto dto)
    {
        try
        {
            return Ok(await _discordService.CreateFlaggedUserAsync(dto));
        }
        catch (Exception ex)
        {
            return Problem("500", ex.Message);
        }
    }

    [HttpGet]
    [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any, NoStore = false)]
    public async Task<ActionResult<bool>> GetFlaggedUsersAsync()
    {
        try
        {
            return Ok(await _discordService.GetFlaggedUsersAsync());
        }
        catch (Exception ex)
        {
            return Problem("500", ex.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<bool>> DeleteFlaggedUserAsync(ulong userId)
    {
        try
        {
            return Ok(await _discordService.DeleteFlaggedUserAsync(userId));
        }
        catch (Exception ex)
        {
            return Problem("500", ex.Message);
        }
    }
    
    [HttpGet]
    public async Task<ActionResult<bool>> GuidIsBeingCheckedForTwitterLinksAsync(ulong guildId)
    {
        try
        {
            return Ok(await _discordService.GuidIsBeingCheckedForTwitterLinksAsync(guildId));
        }
        catch (Exception ex)
        {
            return Problem("500", ex.Message);
        }
    }
}