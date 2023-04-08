using IPBot.Infrastructure.Interfaces;

namespace IPBot.DataServices;

public class IPService : IIPService
{
    private const string BaseUri = "IP";
    private readonly HttpClient _httpClient;

    public IPService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetCurrentDomainAsync()
    {
        return await _httpClient.GetStringAsync($"{BaseUri}/GetCurrentServerDomain");
    }

    public async Task<string> GetLocalIPAsync()
    {
        return await _httpClient.GetStringAsync($"{BaseUri}/GetLocalIP");
    }

    public async Task<string> GetServerIPAsync()
    {
        return await _httpClient.GetStringAsync($"{BaseUri}/GetServerIP");
    }
}