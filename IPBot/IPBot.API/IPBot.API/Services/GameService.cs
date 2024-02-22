using AutoMapper;
using IPBot.API.Domain.Entities;
using IPBot.API.Domain.Interfaces;
using IPBot.API.Domain.Utilities;
using IPBot.Shared.Dtos;
using IPBot.Shared.Services;

namespace IPBot.API.Services;

public class GameService(IMapper mapper, IIPService ipService, IGameRepository gameRepository, IGameServerRepository gameServerRepository) : IGameService
{
    public async Task<ServerInfoDto> GetMinecraftServerStatusAsync(int portNumber)
    {
        return await GetServerInfoAsync(portNumber);
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