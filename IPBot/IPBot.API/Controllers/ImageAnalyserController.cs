using IPBot.Common.Dtos;
using IPBot.Common.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace IPBot.API.Controllers;

[Authorize]
public class ImageAnalyserController : MainController
{
    private readonly IImageAnalyserService _animeAnalyserService;

    public ImageAnalyserController(IImageAnalyserService animeAnalyserService)
    {
        _animeAnalyserService = animeAnalyserService;
    }

    [HttpGet("{encodedUrl}")]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, NoStore = false)]
    public async Task<ActionResult<double>> GetAnimeScoreAsync(string encodedUrl)
    {
        try
        {
            var decodedUrl = Base64UrlEncoder.Decode(encodedUrl);
            return Ok(await _animeAnalyserService.GetAnimeScoreAsync(decodedUrl));
        }
        catch (Exception ex)
        {
            return Problem("500", ex.Message);
        }
    }

    [HttpGet("{encodedUrl}")]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, NoStore = false)]
    public async Task<ActionResult<List<CategoryAnalysisDto>>> GetContentSafetyAnalysisAsync(string encodedUrl)
    {
        try
        {
            var decodedUrl = Base64UrlEncoder.Decode(encodedUrl);
            return Ok(await _animeAnalyserService.GetContentSafetyAnalysisAsync(decodedUrl));
        }
        catch (Exception ex)
        {
            return Problem("500", ex.Message);
        }
    }
}
