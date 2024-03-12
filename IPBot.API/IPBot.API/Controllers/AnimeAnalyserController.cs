using IPBot.Common.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace IPBot.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]/[action]")]
public class AnimeAnalyserController(IAnimeAnalyserService animeAnalyserService) : ControllerBase
{
    [HttpGet("{encodedUrl}")]
    public async Task<double> GetAnimeScoreAsync(string encodedUrl)
    {
        var decodedUrl = Base64UrlEncoder.Decode(encodedUrl);
        return await animeAnalyserService.GetAnimeScoreAsync(decodedUrl);
    }
}
