using AutoMapper;
using IPBot.API.Domain.Entities;
using IPBot.API.Domain.Interfaces;
using IPBot.API.Domain.Utilities;
using IPBot.Shared.Dtos;
using IPBot.Shared.Services;

namespace IPBot.API.Services;

public class GameService : IGameService
{
    private readonly IMapper _mapper;
    private readonly IIPService _ipService;
    private readonly IGameRepository _gameRepository;
    private readonly IGameServerRepository _gameServerRepository;

    public GameService(IMapper mapper, IIPService ipService, IGameRepository gameRepository, IGameServerRepository gameServerRepository)
    {
        _mapper = mapper;
        _ipService = ipService;
        _gameRepository = gameRepository;
        _gameServerRepository = gameServerRepository;
    }

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
        var game = await _gameRepository.GetWhereAsync(x => x.ShortName == gameName, x => x.GameServers);

        var gameServers = game.GameServers.Where(x => x.Active);

        return _mapper.Map<List<GameServerDto>>(gameServers);
    }

    public async Task<bool> UpdateGameServerInformationAsync(GameServerDto dto)
    {
        var gameServer = _mapper.Map<GameServer>(dto);

        return await _gameServerRepository.UpdateAsync(gameServer);
    }

    private async Task<ServerInfoDto> GetServerInfoAsync(int port)
    {
        var serverIP = await _ipService.GetServerIPAsync();
        serverIP = "5.252.102.179";
        port = 29008;
        var serverInfo = await A2SHelper.SendA2SRequestsAsync($"{serverIP}:{port}");

        return _mapper.Map<ServerInfoDto>(serverInfo);
    }
}