using AutoMapper;
using IPBot.API.Shared.Services;
using IPBot.DataServices.Interfaces.DataServices;
using IPBot.DataServices.Models;
using IPBot.DTOs.Dtos;
using IPBot.Infrastructure.Helpers;

namespace IPBot.DataServices.Service;

public class GameService : IGameService
{
    private readonly IMapper _mapper;
    private readonly IGameDataService _gameDataService;
    private readonly IGameServerDataService _gameServerDataService;

    public GameService(IMapper mapper, IGameDataService gameDataService, IGameServerDataService gameServerDataService)
	{
        _mapper = mapper;
        _gameDataService = gameDataService;
        _gameServerDataService = gameServerDataService;
    }

    public async Task<ServerInfoDto> GetMinecraftServerStatusAsync(int portNumber)
    {
        var serverInfo = await ServerStatusHelper.GetServerInfoAsync(Constants.MinecraftServerCode, portNumber);

        return _mapper.Map<ServerInfoDto>(serverInfo);
    }

    public async Task<ServerInfoDto> GetSteamServerStatusAsync(int portNumber)
    {
        var serverInfo = await ServerStatusHelper.GetServerInfoAsync(Constants.SteamServerCode, portNumber);

        return _mapper.Map<ServerInfoDto>(serverInfo);
    }

    public async Task<List<GameServerDto>> GetActiveServersAsync(string gameName)
    {
        var game = await _gameDataService.GetByShortNameAsync(gameName);

        var gameServers = game.GameServers.Where(x => x.Active);

        return _mapper.Map<List<GameServerDto>>(gameServers);
    }

    public async Task<bool> UpdateGameServerInformationAsync(GameServerDto dto)
    {
        var gameServer = _mapper.Map<GameServer>(dto);

        return await _gameServerDataService.UpdateAsync(gameServer);
    }
}