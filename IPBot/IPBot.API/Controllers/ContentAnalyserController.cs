using System.Text;
using IPBot.Common.Dtos;
using IPBot.Common.Services;
using Microsoft.AspNetCore.Authorization;

namespace IPBot.API.Controllers;

[Authorize]
public class ContentAnalyserController : MainController
{
    private readonly IContentAnalyserService _animeAnalyserService;

    public ContentAnalyserController(IContentAnalyserService animeAnalyserService)
    {
        _animeAnalyserService = animeAnalyserService;
    }

    [HttpGet("{encodedUrl}")]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, NoStore = false)]
    public async Task<ActionResult<double>> GetAnimeScoreAsync(string encodedUrl)
    {
        try
        {
            var decodedUrl = DecodeBase64String(encodedUrl);

            return Ok(await _animeAnalyserService.GetAnimeScoreAsync(decodedUrl));
        }
        catch (Exception ex)
        {
            return Problem("500", ex.Message);
        }
    }

    [HttpGet("{encodedUrl}")]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, NoStore = false)]
    public async Task<ActionResult<List<CategoryAnalysisDto>>> GetImageContentSafetyAnalysisAsync(string encodedUrl)
    {
        try
        {
            var decodedUrl = DecodeBase64String(encodedUrl);

            return Ok(await _animeAnalyserService.GetImageContentSafetyAnalysisAsync(decodedUrl));
        }
        catch (Exception ex)
        {
            return Problem("500", ex.Message);
        }
    }

    [HttpGet("{text}")]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, NoStore = false)]
    public async Task<ActionResult<List<CategoryAnalysisDto>>> GetTextContentSafetyAnalysisAsync(string text)
    {
        try
        {
            return Ok(await _animeAnalyserService.GetTextContentSafetyAnalysisAsync(text));
        }
        catch (Exception ex)
        {
            return Problem("500", ex.Message);
        }
    }

    private static string DecodeBase64String(string base64string)
    {
        var data = Convert.FromBase64String(base64string);
        return Encoding.UTF8.GetString(data);
    }
}
