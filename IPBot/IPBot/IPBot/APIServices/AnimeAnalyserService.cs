using System.Net.Http.Json;
using IPBot.Shared.Services;

namespace IPBot.APIServices;

public class AnimeAnalyserService : IAnimeAnalyserService
{
    private const string BaseUri = "AnimeAnalyser";
    private readonly HttpClient _httpClient;

    public AnimeAnalyserService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<double> GetAnimeScoreAsync(string imageUrl)
    {
        return await _httpClient.GetFromJsonAsync<double>($"{BaseUri}/GetAnimeScore/{imageUrl}");
    }
}
