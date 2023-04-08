using System.Net.Http.Json;
using IPBot.API.Shared.Services;
using IPBot.DTOs.Dtos;

namespace IPBot.APIServices;

public class GameService : IGameService
{
    private const string BaseUri = "GameServer";
    private readonly HttpClient _httpClient;

    public GameService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ServerInfoDto> GetMinecraftServerStatusAsync(int portNumber)
    {
        return await _httpClient.GetFromJsonAsync<ServerInfoDto>($"{BaseUri}/GetMinecraftServerStatus/{portNumber}");
    }

    public async Task<ServerInfoDto> GetSteamServerStatusAsync(int portNumber)
    {
        return await _httpClient.GetFromJsonAsync<ServerInfoDto>($"{BaseUri}/GetSteamServerStatus/{portNumber}");
    }

    public async Task<List<GameServerDto>> GetActiveServersAsync(string gameName)
    {
        return await _httpClient.GetFromJsonAsync<List<GameServerDto>>($"{BaseUri}/GetActiveServers/{gameName}");
    }
}