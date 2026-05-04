using IPBot.Common.Dtos;

namespace IPBot.Common.Services;

public interface IContentAnalyserService
{
    Task<double> GetAnimeScoreAsync(string imageUrl);
    Task<List<CategoryAnalysisDto>> GetImageContentSafetyAnalysisAsync(string imageUrl);
    Task<List<CategoryAnalysisDto>> GetTextContentSafetyAnalysisAsync(string text);
}
