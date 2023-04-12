using AutoMapper;
using IPBot.API.DataServices.Models;
using IPBot.Shared.Dtos;

namespace IPBot.API.Business.AutoMapper;

public class GameProfile : Profile
{
    public GameProfile()
    {
        CreateMap<GameServer, GameServerDto>();
    }
}

