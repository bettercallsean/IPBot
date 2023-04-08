namespace IPBot.API.Shared.Services;

public interface IAnimeAnalyserService
{
    Task<double> GetAnimeScoreAsync(string imageUrl);
}
