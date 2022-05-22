using System.Net.Http;
using System.Net.Http.Json;
using IPBot.Infrastructure.Interfaces;
using IPBot.Infrastructure.Models;

namespace IPBot.DataServices;

public class GameServerDataService : IGameServerService
{
    private const string BaseUri = "GameServer";
    private readonly HttpClient _httpClient;

    public GameServerDataService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<ServerInfo> GetMinecraftServerStatusAsync(int portNumber)
    {
        return await _httpClient.GetFromJsonAsync<ServerInfo>($"{BaseUri}/GetMinecraftServerStatus/{portNumber}");
    }

    public async Task<ServerInfo> GetSteamServerStatusAsync(int portNumber)
    {
        return await _httpClient.GetFromJsonAsync<ServerInfo>($"{BaseUri}/GetSteamServerStatus/{portNumber}");
    }
}