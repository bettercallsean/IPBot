using AutoMapper;
using IPBot.API.Repositories.Models;
using IPBot.Shared.Dtos;

namespace IPBot.API.AutoMapper;

public class GameProfile : Profile
{
    public GameProfile()
    {
        CreateMap<GameServer, GameServerDto>();
    }
}

