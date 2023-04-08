using AutoMapper;
using IPBot.API.Shared.Dtos;
using IPBot.DataServices.Models;

namespace IPBot.API.Business.AutoMapper;

public class GameProfile : Profile
{
    public GameProfile()
    {
        CreateMap<GameServer, GameServerDto>();
    }
}

