namespace IPBot.Infrastructure.Interfaces;

public interface IAnimeAnalyser
{
    Task<double> GetAnimeScoreAsync(string imageUrl);
}
