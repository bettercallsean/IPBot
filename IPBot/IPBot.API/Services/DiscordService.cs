using AutoMapper;
using IPBot.API.Domain.Entities;
using IPBot.API.Domain.Interfaces;
using IPBot.Common.Dtos;
using IPBot.Common.Services;

namespace IPBot.API.Services;

public class DiscordService(IMapper mapper, IDiscordChannelRepository discordChannelRepository, IFlaggedUserRepository flaggedUserRepository, IDiscordGuildRepository discordGuildRepository) : IDiscordService
{
    private readonly IMapper _mapper = mapper;
    private readonly IDiscordChannelRepository _discordChannelRepository = discordChannelRepository;
    private readonly IFlaggedUserRepository _flaggedUserRepository = flaggedUserRepository;
    private readonly IDiscordGuildRepository _discordGuildRepository = discordGuildRepository;

    public async Task<List<DiscordChannelDto>> GetActiveDiscordChannelsAsync()
    {
        var channels = await _discordChannelRepository.GetAllWhereAsync(x => x.UseForBotMessages);

        return _mapper.Map<List<DiscordChannelDto>>(channels);
    }

    public async Task<DiscordChannelDto> GetDiscordChannelAsync(ulong guildId, ulong channelId)
    {
        var discordChannel = await _discordChannelRepository.GetWhereAsync(x => x.GuildId == guildId && x.Id == channelId);

        return _mapper.Map<DiscordChannelDto>(discordChannel);
    }

    public async Task<FlaggedUserDto> GetFlaggedUserAsync(ulong userId)
    {
        var user = await _flaggedUserRepository.GetByIdAsync(userId);

        return _mapper.Map<FlaggedUserDto>(user);
    }

    public async Task<bool> IncrementUserFlaggedCountAsync(ulong userId)
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

    public async Task<DiscordGuildDto> GetDiscordGuildAsync(ulong guildId)
    {
        var guild = await _discordGuildRepository.GetByIdAsync(guildId);

        return _mapper.Map<DiscordGuildDto>(guild);
    }

    public async Task<bool> ToggleTwitterLinkScanningAsync(ulong guildId)
    {
        var guild = await _discordGuildRepository.GetByIdAsync(guildId);

        guild.CheckForTwitterLinks = !guild.CheckForTwitterLinks;

        return await _discordGuildRepository.UpdateAsync(guild);
    }
}