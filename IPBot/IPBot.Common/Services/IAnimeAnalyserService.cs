namespace IPBot.Common.Services;

public interface IAnimeAnalyserService
{
    Task<double> GetAnimeScoreAsync(string imageUrl);
}
