using AutoMapper;
using IPBot.DataServices.Models;
using IPBot.DTOs.Dtos;

namespace IPBot.DataServices.AutoMapper;

public class GameProfile : Profile
{
    public GameProfile()
    {
        CreateMap<GameServer, GameServerDto>();
    }
}

