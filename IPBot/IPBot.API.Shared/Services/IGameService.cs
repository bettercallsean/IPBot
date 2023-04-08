using IPBot.DTOs.Dtos;

namespace IPBot.API.Shared.Services;

public interface IGameService
{
    Task<ServerInfoDto> GetMinecraftServerStatusAsync(int portNumber);
    Task<ServerInfoDto> GetSteamServerStatusAsync(int portNumber);
    Task<List<GameServerDto>> GetActiveServersAsync(string gameName);
    Task<bool> UpdateGameServerInformationAsync(GameServerDto dto);
}
