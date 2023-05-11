using AutoMapper;
using IPBot.API.Repositories.Models;
using IPBot.Shared.Dtos;

namespace IPBot.API.AutoMapper;

public class GameServerProfile : Profile
{
    public GameServerProfile()
    {
        CreateMap<GameServerDto, GameServer>();
    }
}