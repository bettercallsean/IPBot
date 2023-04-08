using AutoMapper;
using IPBot.DataServices.Models;
using IPBot.DTOs.Dtos;

namespace IPBot.DataServices.AutoMapper;

public class GameServerProfile : Profile
{
    public GameServerProfile()
    {
        CreateMap<GameServerDto, GameServer>();
    }
}