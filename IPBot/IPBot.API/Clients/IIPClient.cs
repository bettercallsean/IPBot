using IPBot.API.Clients.Interfaces;

namespace IPBot.API.Clients;

internal class IPClient(HttpClient httpClient) : IIPClient
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<string> GetLocalIPAsync()
    {
        var response = await _httpClient.GetAsync(string.Empty);
        response.EnsureSuccessStatusCode();

        var ip = await response.Content.ReadAsStringAsync();
        return ip;
    }
}
