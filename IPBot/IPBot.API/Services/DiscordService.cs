using AutoMapper;
using IPBot.API.Domain.Entities;
using IPBot.API.Domain.Interfaces;
using IPBot.Common.Dtos;
using IPBot.Common.Services;

namespace IPBot.API.Services;

public class DiscordService : IDiscordService
{
    private readonly IMapper _mapper;
    private readonly IDiscordChannelRepository _discordChannelRepository;
    private readonly IFlaggedUserRepository _flaggedUserRepository;
    private readonly IDiscordGuildRepository _discordGuildRepository;

    public DiscordService(IMapper mapper, IDiscordChannelRepository discordChannelRepository, IFlaggedUserRepository flaggedUserRepository, IDiscordGuildRepository discordGuildRepository)
    {
        _mapper = mapper;
        _discordChannelRepository = discordChannelRepository;
        _flaggedUserRepository = flaggedUserRepository;
        _discordGuildRepository = discordGuildRepository;
    }

    public async Task<List<DiscordChannelDto>> GetInUseDiscordChannelsAsync()
    {
        var channels = await _discordChannelRepository.GetAllWhereAsync(x => x.UseForBotMessages);

        return _mapper.Map<List<DiscordChannelDto>>(channels);
    }

    public async Task<bool> ChannelIsBeingAnalysedForAnimeAsync(ulong guildId, ulong channelId)
    {
        var discordChannel = await _discordChannelRepository.GetWhereAsync(x => x.GuildId == guildId
                                                                                && x.Id == channelId);

        return discordChannel?.AnalyseForAnime == true;
    }

    public async Task<FlaggedUserDto> GetFlaggedUserAsync(ulong userId)
    {
        var user = await _flaggedUserRepository.GetByIdAsync(userId);

        return _mapper.Map<FlaggedUserDto>(user);
    }

    public async Task<bool> UpdateUserFlaggedCountAsync(ulong userId)
    {
        var user = await _flaggedUserRepository.GetByIdAsync(userId);

        user.FlaggedCount++;

        return await _flaggedUserRepository.UpdateAsync(user);
    }

    public async Task<bool> CreateFlaggedUserAsync(FlaggedUserDto dto)
    {
        var flaggedUser = _mapper.Map<FlaggedUser>(dto);

        return await _flaggedUserRepository.AddAsync(flaggedUser);
    }

    public async Task<List<FlaggedUserDto>> GetFlaggedUsersAsync()
    {
        var users = await _flaggedUserRepository.GetAllAsync();

        return _mapper.Map<List<FlaggedUserDto>>(users);
    }

    public async Task<bool> DeleteFlaggedUserAsync(ulong userId)
    {
        var user = await _flaggedUserRepository.GetByIdAsync(userId);

        return await _flaggedUserRepository.DeleteAsync(user);
    }

    public async Task<bool> GuidIsBeingCheckedForTwitterLinksAsync(ulong guildId)
    {
        var guild = await _discordGuildRepository.GetByIdAsync(guildId);

        return guild.CheckForTwitterLinks;
    }
}