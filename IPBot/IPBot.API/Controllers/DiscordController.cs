using IPBot.Common.Dtos;
using IPBot.Common.Services;
using Microsoft.AspNetCore.Authorization;

namespace IPBot.API.Controllers;

[Authorize]
public class DiscordController(IDiscordService discordService) : MainController
{
    private readonly IDiscordService _discordService = discordService;

    [HttpGet("channels/active")]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, NoStore = false)]
    public async Task<ActionResult<List<DiscordChannelDto>>> GetActiveDiscordChannelsAsync()
    {
        try
        {
            return Ok(await _discordService.GetActiveDiscordChannelsAsync());
        }
        catch (Exception ex)
        {
            return Problem("500", ex.Message);
        }
    }

    [HttpGet("guilds/{guildId:long}/channels/{channelId:long}/analyse-for-anime")]
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

    [HttpGet("users/flagged/{userId:long}")]
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

    [HttpPatch("users/flagged/{userId:long}/increment")]
    public async Task<ActionResult<bool>> IncrementUserFlaggedCountAsync(ulong userId)
    {
        try
        {
            return Ok(await _discordService.IncrementUserFlaggedCountAsync(userId));
        }
        catch (Exception ex)
        {
            return Problem("500", ex.Message);
        }
    }

    [HttpPost("users/flagged")]
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

    [HttpGet("users/flagged")]
    [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any, NoStore = false)]
    public async Task<ActionResult<List<FlaggedUserDto>>> GetFlaggedUsersAsync()
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

    [HttpDelete("users/flagged/{userId:long}")]
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

    [HttpGet("guilds/{guildId:long}/twitter-links")]
    public async Task<ActionResult<bool>> GuildIsBeingCheckedForTwitterLinksAsync(ulong guildId)
    {
        try
        {
            return Ok(await _discordService.GuildIsBeingCheckedForTwitterLinksAsync(guildId));
        }
        catch (Exception ex)
        {
            return Problem("500", ex.Message);
        }
    }

    [HttpPatch("guilds/{guildId:long}/twitter-links")]
    public async Task<ActionResult<bool>> ToggleTwitterLinkScanningAsync(ulong guildId)
    {
        try
        {
            return Ok(await _discordService.ToggleTwitterLinkScanningAsync(guildId));
        }
        catch (Exception ex)
        {
            return Problem("500", ex.Message);
        }
    }
}