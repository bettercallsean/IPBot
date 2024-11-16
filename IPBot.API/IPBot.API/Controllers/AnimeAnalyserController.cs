using IPBot.Common.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace IPBot.API.Controllers;

[Authorize]
public class AnimeAnalyserController(IAnimeAnalyserService animeAnalyserService) : MainController
{
    [HttpGet("{encodedUrl}")]
    public async Task<ActionResult<double>> GetAnimeScoreAsync(string encodedUrl)
    {
        try
        {
            var decodedUrl = Base64UrlEncoder.Decode(encodedUrl);
            return Ok(await animeAnalyserService.GetAnimeScoreAsync(decodedUrl));
        }
        catch(Exception ex)
        {
            return Problem("500", ex.Message);
        }
    }
}
