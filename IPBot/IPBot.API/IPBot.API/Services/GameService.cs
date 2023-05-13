﻿using System.Text.Json;
using System.Text.RegularExpressions;
using AutoMapper;
using IPBot.API.Domain.Entities;
using IPBot.API.Domain.Interfaces;
using IPBot.Infrastructure.Helpers;
using IPBot.Shared.Dtos;
using IPBot.Shared.Services;

namespace IPBot.API.Services;

public partial class GameService : IGameService
{
    private const string ServerStatusScriptName = "get_server_status.py";
    private const string SteamServerCode = "steam";
    private const string MinecraftServerCode = "mc";
    
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
        return await GetServerInfoAsync(MinecraftServerCode, portNumber);
    }

    public async Task<ServerInfoDto> GetSteamServerStatusAsync(int portNumber)
    {
        return await GetServerInfoAsync(SteamServerCode, portNumber);
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
    
    private async Task<ServerInfoDto> GetServerInfoAsync(string gameCode, int port)
    {
        var serverIP = await _ipService.GetServerIPAsync();
        var serverInfo = await GetServerInfoJsonAsync(gameCode, serverIP, port);

        return ParseServerInfoJson(serverInfo);
    }

    private static async Task<string> GetServerInfoJsonAsync(string gameCode, string serverIP, int portNumber)
    {
        var serverResults =
            await PythonScriptHelper.RunPythonScriptAsync(ServerStatusScriptName, $"{gameCode} {serverIP} {portNumber}");

        return serverResults;
    }

    private static ServerInfoDto ParseServerInfoJson(string serverInfo)
    {
        var serverInfoModel = (string.IsNullOrWhiteSpace(serverInfo)
            ? new ServerInfoDto()
            : JsonSerializer.Deserialize<ServerInfoDto>(serverInfo))!;
        
        if (!string.IsNullOrWhiteSpace(serverInfoModel.Map))
            serverInfoModel.Map = string.Join(" ", CapitalLetterRegex().Split(serverInfoModel.Map));

        return serverInfoModel;
    }

    [GeneratedRegex("(?<!^)(?=[A-Z])")]
    private static partial Regex CapitalLetterRegex();
}