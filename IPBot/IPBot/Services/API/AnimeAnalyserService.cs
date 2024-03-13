using IPBot.Common.Services;
using RestSharp;

namespace IPBot.Services.API;

public class AnimeAnalyserService(IRestClient client, IConfiguration configuration) : ServiceBase(client, configuration), IAnimeAnalyserService
{
    private const string BaseUri = "/AnimeAnalyser";

    public async Task<double> GetAnimeScoreAsync(string imageUrl)
    {
        return await GetAsync<double>($"{BaseUri}/GetAnimeScore/{imageUrl}");
    }
}
