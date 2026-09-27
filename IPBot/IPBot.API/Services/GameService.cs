using AutoMapper;
using IPBot.API.Clients.Interfaces;
using IPBot.API.Domain.Entities;
using IPBot.API.Domain.Interfaces;
using IPBot.API.Domain.Utilities;
using IPBot.Common.Dtos;
using IPBot.Common.Services;

namespace IPBot.API.Services;

internal class GameService(IMapper mapper, IIPService ipService, IGameRepository gameRepository, IGameServerRepository gameServerRepository, IMinecraftClient minecraftClient) : IGameService
{
    private readonly IMapper _mapper = mapper;
    private readonly IIPService _ipService = ipService;
    private readonly IGameRepository _gameRepository = gameRepository;
    private readonly IGameServerRepository _gameServerRepository = gameServerRepository;
    private readonly IMinecraftClient _minecraftClient = minecraftClient;

    public async Task<ServerInfoDto> GetMinecraftServerStatusAsync(int portNumber)
    {
        var serverIP = await _ipService.GetServerIPAsync();
        var minecraftServerInfo = await _minecraftClient.GetServerStatusAsync(serverIP, portNumber);

        return _mapper.Map<ServerInfoDto>(minecraftServerInfo);
    }

    public async Task<ServerInfoDto> GetSteamServerStatusAsync(int portNumber)
    {
        return await GetServerInfoAsync(portNumber);
    }

    public async Task<List<GameServerDto>> GetActiveServersAsync(string gameName)
    {
        var game = await _gameRepository.GetWhereAsync(x => x.ShortName == gameName, x => x.GameServers);
        var gameServers = game.GameServers.Where(x => x.Active);

        return _mapper.Map<List<GameServerDto>>(gameServers);
    }

    public async Task<bool> UpdateGameServerAsync(GameServerDto dto)
    {
        var gameServer = _mapper.Map<GameServer>(dto);

        return await _gameServerRepository.UpdateAsync(gameServer);
    }

    private async Task<ServerInfoDto> GetServerInfoAsync(int port)
    {
        var serverIP = await _ipService.GetServerIPAsync();
        var serverInfo = await A2SHelper.SendA2SRequestsAsync($"{serverIP}:{port}");

        return _mapper.Map<ServerInfoDto>(serverInfo);
    }
}