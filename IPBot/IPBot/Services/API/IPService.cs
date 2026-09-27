using IPBot.Common.Services;
using RestSharp;

namespace IPBot.Services.API;

internal class IPService(IRestClient client) : RestService(client), IIPService
{
    private const string BaseUri = "/IP";

    public async Task<string> GetCurrentServerDomainAsync()
    {
        return await GetAsync<string>($"{BaseUri}/current-domain");
    }

    public async Task<string> GetLocalIPAsync()
    {
        return await GetAsync<string>($"{BaseUri}/local");
    }

    public async Task<string> GetServerIPAsync()
    {
        return await GetAsync<string>($"{BaseUri}/server");
    }

    public Task<bool> UpdateServerIPAsync(string ip)
    {
        return PatchAsync<bool>($"{BaseUri}/server", new { ip });
    }
}