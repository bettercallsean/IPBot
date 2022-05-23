using System.Net.Http;
using IPBot.Infrastructure.Interfaces;

namespace IPBot.DataServices;

public class IPDataService : IIPService
{    
    private const string BaseUri = "IP";
    private readonly HttpClient _httpClient;

    public IPDataService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<string> GetCurrentDomainAsync()
    {
        return await _httpClient.GetStringAsync($"{BaseUri}/GetCurrentServerDomain");
    }

    public async Task<string> GetCurrentIPAsync()
    {
        return await _httpClient.GetStringAsync($"{BaseUri}/GetCurrentIP");
    }
}