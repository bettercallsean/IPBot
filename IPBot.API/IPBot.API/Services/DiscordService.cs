using AutoMapper;
using IPBot.API.Domain.Interfaces;
using IPBot.Common.Dtos;
using IPBot.Common.Services;

namespace IPBot.API.Services;

public class DiscordService(IMapper mapper, IDiscordChannelRepository discordChannelRepository) : IDiscordService
{
    public async Task<List<DiscordChannelDto>> GetInUseDiscordChannelsAsync()
    {
        var channels = await discordChannelRepository.GetAllWhereAsync(x => x.UseForBotMessages);

        return mapper.Map<List<DiscordChannelDto>>(channels);
    }

    public async Task<bool> ChannelIsBeingAnalysedForAnimeAsync(ulong guildId, ulong channelId)
    {
        var discordChannel = await discordChannelRepository.GetWhereAsync(x => x.GuildId == guildId
                                                                                && x.Id == channelId);

        return discordChannel?.AnalyseForAnime == true;
    }
}