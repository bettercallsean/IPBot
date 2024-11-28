using IPBot.Common.Services;
using IPBot.Configuration;
using RestSharp;

namespace IPBot.Services.API;

public class AnimeAnalyserService(IRestClient client, BotConfiguration botConfiguration) : ServiceBase(client, botConfiguration.APILogin), IAnimeAnalyserService
{
    private const string BaseUri = "/AnimeAnalyser";

    public async Task<double> GetAnimeScoreAsync(string imageUrl)
    {
        return await GetAsync<double>($"{BaseUri}/GetAnimeScore/{imageUrl}");
    }
}
