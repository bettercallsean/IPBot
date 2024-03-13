using IPBot.Common.Dtos;
using IPBot.Common.Services;
using RestSharp;

namespace IPBot.Services.API;

public class DiscordService(IRestClient client, IConfiguration configuration) : ServiceBase(client, configuration), IDiscordService
{
    private const string BaseUri = "/Discord";

    public async Task<List<DiscordChannelDto>> GetInUseDiscordChannelsAsync()
    {
        return await GetAsync<List<DiscordChannelDto>>($"{BaseUri}/GetInUseDiscordChannels");
    }
}