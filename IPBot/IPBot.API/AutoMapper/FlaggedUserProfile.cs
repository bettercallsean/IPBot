using AutoMapper;
using IPBot.API.Domain.Entities;
using IPBot.Common.Dtos;

namespace IPBot.API.AutoMapper;

public class FlaggedUserProfile : Profile
{
    public FlaggedUserProfile()
    {
        CreateMap<FlaggedUser, FlaggedUserDto>();
        CreateMap<FlaggedUserDto, FlaggedUser>();
    }
}