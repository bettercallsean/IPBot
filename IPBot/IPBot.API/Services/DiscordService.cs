using AutoMapper;
using IPBot.API.Domain.Entities;
using IPBot.API.Domain.Interfaces;
using IPBot.API.Domain.Repositories;
using IPBot.Common.Dtos;
using IPBot.Common.Services;

namespace IPBot.API.Services;

public class DiscordService(IMapper mapper, IDiscordChannelRepository discordChannelRepository, IFlaggedUserRepository flaggedUserRepository, IDiscordGuildRepository discordGuildRepository) : IDiscordService
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

    public async Task<FlaggedUserDto> GetFlaggedUserAsync(ulong userId)
    {
        var user = await flaggedUserRepository.GetByIdAsync(userId);

        return mapper.Map<FlaggedUserDto>(user);
    }

    public async Task<bool> UpdateUserFlaggedCountAsync(ulong userId)
    {
        var user = await flaggedUserRepository.GetByIdAsync(userId);

        user.FlaggedCount++;

        return await flaggedUserRepository.UpdateAsync(user);
    }

    public async Task<bool> CreateFlaggedUserAsync(FlaggedUserDto dto)
    {
        var flaggedUser = mapper.Map<FlaggedUser>(dto);

        return await flaggedUserRepository.AddAsync(flaggedUser);
    }

    public async Task<List<FlaggedUserDto>> GetFlaggedUsersAsync()
    {
        var users = await flaggedUserRepository.GetAllAsync();

        return mapper.Map<List<FlaggedUserDto>>(users);
    }

    public async Task<bool> DeleteFlaggedUserAsync(ulong userId)
    {
        var user = await flaggedUserRepository.GetByIdAsync(userId);

        return await flaggedUserRepository.DeleteAsync(user);
    }

    public async Task<bool> GuidIsBeingCheckedForTwitterLinksAsync(ulong guildId)
    {
        var guild = await discordGuildRepository.GetByIdAsync(guildId);

        return guild.CheckForTwitterLinks;
    }
}