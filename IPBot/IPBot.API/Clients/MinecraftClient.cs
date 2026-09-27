using IPBot.API.Clients.Interfaces;
using IPBot.Common.Dtos;

namespace IPBot.API.Clients;

internal class MinecraftClient(HttpClient httpClient) : IMinecraftClient
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<ServerInfoDto> GetServerStatusAsync(string ip, int port)
    {
        var response = await _httpClient.GetAsync($"https://api.mcstatus.io/v2/status/java/{ip}:{port}");
        response.EnsureSuccessStatusCode();

        var serverInfo = await response.Content.ReadFromJsonAsync<ServerInfoDto>();

        return serverInfo ?? throw new InvalidOperationException("Failed to deserialize server info.");
    }
}
