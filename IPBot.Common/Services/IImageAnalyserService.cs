using IPBot.Common.Dtos;

namespace IPBot.Common.Services;

public interface IImageAnalyserService
{
    Task<double> GetAnimeScoreAsync(string imageUrl);
    Task<List<CategoryAnalysisDto>> GetContentSafetyAnalysisAsync(string imageUrl);
}
