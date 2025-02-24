using AutoMapper;
using IPBot.API.Constants;
using IPBot.API.Domain.Entities;
using IPBot.API.Domain.Interfaces;
using IPBot.API.Domain.Models;
using IPBot.API.Domain.Utilities;
using IPBot.Common.Dtos;
using IPBot.Common.Services;

namespace IPBot.API.Services;

public class GameService(IMapper mapper, IIPService ipService, IGameRepository gameRepository, IGameServerRepository gameServerRepository, IHttpClientFactory httpClientFactory) : IGameService
{
    public async Task<ServerInfoDto> GetMinecraftServerStatusAsync(int portNumber)
    {
        var serverIP = await ipService.GetServerIPAsync();
        var httpClient = httpClientFactory.CreateClient(KeyedHttpClientNames.MinecraftServerClient);
        var minecraftServerInfo = await httpClient.GetFromJsonAsync<MinecraftServerInfo>($"/status/java/{serverIP}:{portNumber}");

        return mapper.Map<ServerInfoDto>(minecraftServerInfo);
    }

    public async Task<ServerInfoDto> GetSteamServerStatusAsync(int portNumber)
    {
        return await GetServerInfoAsync(portNumber);
    }

    public async Task<List<GameServerDto>> GetActiveServersAsync(string gameName)
    {
        var game = await gameRepository.GetWhereAsync(x => x.ShortName == gameName, x => x.GameServers);
        var gameServers = game.GameServers.Where(x => x.Active);

        return mapper.Map<List<GameServerDto>>(gameServers);
    }

    public async Task<bool> UpdateGameServerInformationAsync(GameServerDto dto)
    {
        var gameServer = mapper.Map<GameServer>(dto);

        return await gameServerRepository.UpdateAsync(gameServer);
    }

    private async Task<ServerInfoDto> GetServerInfoAsync(int port)
    {
        var serverIP = await ipService.GetServerIPAsync();
        var serverInfo = await A2SHelper.SendA2SRequestsAsync($"{serverIP}:{port}");

        return mapper.Map<ServerInfoDto>(serverInfo);
    }
}