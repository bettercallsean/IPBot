namespace IPBot.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnimeAnalyserController : ControllerBase, IAnimeAnalyser
{
    private readonly IAnimeAnalyser _animeAnalyser;
    public AnimeAnalyserController(IAnimeAnalyser animeAnalyser)
    {
        _animeAnalyser = animeAnalyser;
    }

    [HttpGet("GetAnimeScore")]
    public async Task<double> GetAnimeScoreAsync(string imageUrl)
    {
        return await _animeAnalyser.GetAnimeScoreAsync(imageUrl);
    }
}
