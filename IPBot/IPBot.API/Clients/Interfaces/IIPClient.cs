namespace IPBot.API.Clients.Interfaces;

internal interface IIPClient
{
    Task<string> GetLocalIPAsync();
}
