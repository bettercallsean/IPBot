using AutoMapper;
using IPBot.API.Domain.Entities;
using IPBot.Common.Dtos;

namespace IPBot.API.AutoMapper;

public class GameServerProfile : Profile
{
    public GameServerProfile()
    {
        CreateMap<GameServerDto, GameServer>();
    }
}