using AutoMapper;
using IPBot.API.Domain.Entities;
using IPBot.Shared.Dtos;

namespace IPBot.API.AutoMapper;

public class DiscordChannelProfile : Profile
{
    public DiscordChannelProfile()
    {
        CreateMap<DiscordChannel, DiscordChannelDto>();
    }
}