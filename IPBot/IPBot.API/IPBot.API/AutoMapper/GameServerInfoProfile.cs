using AutoMapper;
using IPBot.Shared.Dtos;

namespace IPBot.API;

public class GameServerInfoProfile : Profile
{
    public GameServerInfoProfile()
    {
        CreateMap<SteamServerInfo, ServerInfoDto>()
            .ForMember(x => x.ServerName, y => y.MapFrom(z => z.Name))
            .ForMember(x => x.PlayerCount, y => y.MapFrom(z => z.Players));
    }
}
