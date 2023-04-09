﻿using IPBot.Shared.Dtos;
using IPBot.Shared.Services;
using RestSharp;

namespace IPBot.APIServices;

public class GameService : ServiceBase, IGameService
{
    private const string BaseUri = "GameServer";

    public GameService(IRestClient client) : base(client) { }

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