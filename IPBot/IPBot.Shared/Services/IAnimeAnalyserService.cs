namespace IPBot.Shared.Services;

public interface IAnimeAnalyserService
{
    Task<double> GetAnimeScoreAsync(string imageUrl);
}
