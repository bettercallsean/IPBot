using IPBot.Common.Services;
using RestSharp;

namespace IPBot.APIServices;

public class AnimeAnalyserService : ServiceBase, IAnimeAnalyserService
{
    private const string BaseUri = "/AnimeAnalyser";

    public AnimeAnalyserService(IRestClient client, IConfiguration configuration) : base(client, configuration) { }

    public async Task<double> GetAnimeScoreAsync(string imageUrl)
    {
        return await GetAsync<double>($"{BaseUri}/GetAnimeScore/{imageUrl}");
    }
}
