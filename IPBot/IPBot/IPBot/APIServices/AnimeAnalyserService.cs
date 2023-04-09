﻿using System.Net.Http.Json;
using IPBot.Shared.Services;
using RestSharp;

namespace IPBot.APIServices;

public class AnimeAnalyserService : ServiceBase, IAnimeAnalyserService
{
    private const string BaseUri = "AnimeAnalyser";

    public AnimeAnalyserService(IRestClient client) : base(client) { }

    public async Task<double> GetAnimeScoreAsync(string imageUrl)
    {
        return await GetAsync<double>($"{BaseUri}/GetAnimeScore/{imageUrl}");
    }
}
