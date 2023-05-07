using IPBot.Shared.Services;
using RestSharp;

namespace IPBot.APIServices;

public class IPService : ServiceBase, IIPService
{
    private const string BaseUri = "IP";

    public IPService(IRestClient client, IConfiguration configuration) : base(client, configuration)  { }

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