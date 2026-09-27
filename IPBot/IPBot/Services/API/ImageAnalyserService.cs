using IPBot.Common.Dtos;
using IPBot.Common.Services;
using RestSharp;

namespace IPBot.Services.API;

internal class ImageAnalyserService(IRestClient client) : RestService(client), IImageAnalyserService
{
    private const string BaseUri = "/ImageAnalyser";

    public async Task<double> GetAnimeScoreAsync(string imageUrl)
    {
        return await GetAsync<double>($"{BaseUri}/scores/anime/{imageUrl}");
    }

    public async Task<List<CategoryAnalysisDto>> GetContentSafetyAnalysisAsync(string imageUrl)
    {
        return await GetAsync<List<CategoryAnalysisDto>>($"{BaseUri}/content-safety/{imageUrl}");
    }
}
