using IPBot.Common.Dtos;

namespace IPBot.API.Clients.Interfaces;

internal interface IMinecraftClient
{
    Task<ServerInfoDto> GetServerStatusAsync(string ip, int port);
}