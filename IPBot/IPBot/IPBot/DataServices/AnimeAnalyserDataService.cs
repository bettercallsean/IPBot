using System.Net.Http;
using System.Net.Http.Json;
using IPBot.Infrastructure.Interfaces;

namespace IPBot.DataServices;

public class AnimeAnalyserDataService : IAnimeAnalyser
{
    private const string BaseUri = "AnimeAnalyser";
    private readonly HttpClient _httpClient;

    public AnimeAnalyserDataService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<double> GetAnimeScoreAsync(string imageUrl)
    {
        return await _httpClient.GetFromJsonAsync<double>($"{BaseUri}/GetAnimeScore/{imageUrl}");
    }
}
