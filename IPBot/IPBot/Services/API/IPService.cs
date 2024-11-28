using IPBot.Common.Services;
using IPBot.Configuration;
using RestSharp;

namespace IPBot.Services.API;

public class IPService(IRestClient client, BotConfiguration botConfiguration) : ServiceBase(client, botConfiguration.APILogin), IIPService
{
    private const string BaseUri = "/IP";

    public async Task<string> GetCurrentServerDomainAsync()
    {
        return await GetAsync<string>($"{BaseUri}/GetCurrentServerDomain");
    }

    public async Task<string> GetLocalIPAsync()
    {
        return await GetAsync<string>($"{BaseUri}/GetLocalIP");
    }

    public async Task<string> GetServerIPAsync()
    {
        return await GetAsync<string>($"{BaseUri}/GetServerIP");
    }

    public Task<bool> UpdateServerIPAsync(string ip)
    {
        throw new NotImplementedException();
    }
}