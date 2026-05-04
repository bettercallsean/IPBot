using IPBot.Common.Dtos;
using IPBot.Common.Services;
using IPBot.Configuration;
using RestSharp;

namespace IPBot.Services.API;

public class ContentAnalyserService(IRestClient client, BotConfiguration botConfiguration) : ServiceBase(client, botConfiguration.APILogin), IContentAnalyserService
{
    private const string BaseUri = "/ContentAnalyser";

    public async Task<double> GetAnimeScoreAsync(string imageUrl)
    {
        return await GetAsync<double>($"{BaseUri}/GetAnimeScore/{imageUrl}");
    }

    public async Task<List<CategoryAnalysisDto>> GetImageContentSafetyAnalysisAsync(string imageUrl)
    {
        return await GetAsync<List<CategoryAnalysisDto>>($"{BaseUri}/GetImageContentSafetyAnalysis/{imageUrl}");
    }

    public async Task<List<CategoryAnalysisDto>> GetTextContentSafetyAnalysisAsync(string imageUrl)
    {
        return await GetAsync<List<CategoryAnalysisDto>>($"{BaseUri}/GetTextContentSafetyAnalysis/{imageUrl}");
    }
}
