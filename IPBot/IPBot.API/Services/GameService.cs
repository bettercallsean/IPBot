using AutoMapper;
using IPBot.API.Constants;
using IPBot.API.Domain.Entities;
using IPBot.API.Domain.Interfaces;
using IPBot.API.Domain.Models;
using IPBot.API.Domain.Utilities;
using IPBot.Common.Dtos;
using IPBot.Common.Services;

namespace IPBot.API.Services;

public class GameService : IGameService
{
    private readonly IMapper _mapper;
    private readonly IIPService _ipService;
    private readonly IGameRepository _gameRepository;
    private readonly IGameServerRepository _gameServerRepository;
    private readonly IHttpClientFactory _httpClientFactory;

    public GameService(IMapper mapper, IIPService ipService, IGameRepository gameRepository, IGameServerRepository gameServerRepository, IHttpClientFactory httpClientFactory)
    {
        _mapper = mapper;
        _ipService = ipService;
        _gameRepository = gameRepository;
        _gameServerRepository = gameServerRepository;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<ServerInfoDto> GetMinecraftServerStatusAsync(int portNumber)
    {
        var serverIP = await _ipService.GetServerIPAsync();
        var httpClient = _httpClientFactory.CreateClient(KeyedHttpClientNames.MinecraftServerClient);
        var minecraftServerInfo = await httpClient.GetFromJsonAsync<MinecraftServerInfo>($"/status/java/{serverIP}:{portNumber}");

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

    public async Task<bool> UpdateGameServerInformationAsync(GameServerDto dto)
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