using IPBot.Shared.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace IPBot.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]/[action]")]
public class AnimeAnalyserController : ControllerBase
{
    private readonly IAnimeAnalyserService _animeAnalyserService;

    public AnimeAnalyserController(IAnimeAnalyserService animeAnalyserService)
    {
        _animeAnalyserService = animeAnalyserService;
    }

    [HttpGet("{encodedUrl}")]
    public async Task<double> GetAnimeScoreAsync(string encodedUrl)
    {
        var decodedUrl = Base64UrlEncoder.Decode(encodedUrl);
        return await _animeAnalyserService.GetAnimeScoreAsync(decodedUrl);
    }
}
