using IPBot.Common.Dtos;
using IPBot.Common.Services;
using RestSharp;

namespace IPBot.APIServices;

public class GameService : ServiceBase, IGameService
{
    private const string BaseUri = "/GameServer";

    public GameService(IRestClient client, IConfiguration configuration) : base(client, configuration)  { }

    public async Task<ServerInfoDto> GetMinecraftServerStatusAsync(int portNumber)
    {
        return await GetAsync<ServerInfoDto>($"{BaseUri}/GetMinecraftServerStatus/{portNumber}");
    }

    public async Task<ServerInfoDto> GetSteamServerStatusAsync(int portNumber)
    {
        return await GetAsync<ServerInfoDto>($"{BaseUri}/GetSteamServerStatus/{portNumber}");
    }

    public async Task<List<GameServerDto>> GetActiveServersAsync(string gameName)
    {
        return await GetAsync<List<GameServerDto>>($"{BaseUri}/GetActiveServers/{gameName}");
    }

    public async Task<bool> UpdateGameServerInformationAsync(GameServerDto dto)
    {
        return await PostAsync<bool>($"{BaseUri}/UpdateGameServerInformation", dto);
    }
}