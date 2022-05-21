using IPBot.Infrastructure.Models;

namespace IPBot.Infrastructure.Interfaces;

public interface IGameServerService
{
    Task<ServerInfo> GetMinecraftServerStatusAsync(int portNumber);
    Task<ServerInfo> GetSteamServerStatusAsync(int portNumber);
}