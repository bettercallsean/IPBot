using IPBot.Common.Dtos;
using IPBot.Common.Services;
using IPBot.Configuration;
using RestSharp;

namespace IPBot.Services.API;

public class ImageAnalyserService(IRestClient client, BotConfiguration botConfiguration) : ServiceBase(client, botConfiguration.APILogin), IImageAnalyserService
{
    private const string BaseUri = "/ImageAnalyser";

    public async Task<double> GetAnimeScoreAsync(string imageUrl)
    {
        return await GetAsync<double>($"{BaseUri}/GetAnimeScore/{imageUrl}");
    }

    public async Task<List<CategoryAnalysisDto>> GetContentSafetyAnalysisAsync(string imageUrl)
    {
        return await GetAsync<List<CategoryAnalysisDto>>($"{BaseUri}/GetContentSafetyAnalysis/{imageUrl}");
    }
}
