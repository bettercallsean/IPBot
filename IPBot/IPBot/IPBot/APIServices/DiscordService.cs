using IPBot.Common.Dtos;
using IPBot.Common.Services;
using RestSharp;

namespace IPBot.APIServices;

public class DiscordService : ServiceBase, IDiscordService
{
    private const string BaseUri = "/Discord";

    public DiscordService(IRestClient client, IConfiguration configuration) : base(client, configuration)
    {
    }

    public async Task<List<DiscordChannelDto>> GetInUseDiscordChannelsAsync()
    {
        return await GetAsync<List<DiscordChannelDto>>($"{BaseUri}/GetInUseDiscordChannels");
    }
}