namespace IPBot.Common.Services;

public interface IIPService
{
    Task<string> GetCurrentServerDomainAsync();
    Task<string> GetLocalIPAsync();
    Task<string> GetServerIPAsync();
    Task<bool> UpdateServerIPAsync(string ip);
}