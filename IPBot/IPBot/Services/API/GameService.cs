using IPBot.Common.Dtos;
using IPBot.Common.Services;
using IPBot.Configuration;
using RestSharp;

namespace IPBot.Services.API;

public class GameService(IRestClient client, BotConfiguration botConfiguration) : ServiceBase(client, botConfiguration.APILogin), IGameService
{
    private const string BaseUri = "/GameServer";

    public async Task<ServerInfoDto> GetMinecraftServerStatusAsync(int portNumber)
    {
        return await GetAsync<ServerInfoDto>($"{BaseUri}/minecraft/{portNumber}");
    }

    public async Task<ServerInfoDto> GetSteamServerStatusAsync(int portNumber)
    {
        return await GetAsync<ServerInfoDto>($"{BaseUri}/steam/{portNumber}");
    }

    public async Task<List<GameServerDto>> GetActiveServersAsync(string gameName)
    {
        return await GetAsync<List<GameServerDto>>($"{BaseUri}/active/{gameName}");
    }

    public async Task<bool> UpdateGameServerAsync(GameServerDto dto)
    {
        return await PostAsync<bool>($"{BaseUri}", dto);
    }
}