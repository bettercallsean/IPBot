using AutoMapper;
using IPBot.API.Shared.Dtos;
using IPBot.DataServices.Models;

namespace IPBot.API.Business.AutoMapper;

public class GameServerProfile : Profile
{
    public GameServerProfile()
    {
        CreateMap<GameServerDto, GameServer>();
    }
}