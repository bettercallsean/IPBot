using AutoMapper;
using IPBot.API.Domain.Models;
using IPBot.Common.Dtos;

namespace IPBot.API.AutoMapper;

public class GameServerInfoProfile : Profile
{
    public GameServerInfoProfile()
    {
        CreateMap<SteamServerInfo, ServerInfoDto>()
            .ForMember(x => x.ServerName, y => y.MapFrom(z => z.Name))
            .ForMember(x => x.PlayerCount, y => y.MapFrom(z => z.PlayerCount));

        CreateMap<MinecraftServerInfo, ServerInfoDto>()
            .ForMember(x => x.Motd, y => y.MapFrom(z => z.Motd.Clean.Trim()))
            .ForMember(x => x.PlayerCount, y => y.MapFrom(z => z.Players.Online))
            .ForMember(x => x.PlayerNames, y => y.MapFrom(z => z.Players.List.Where(x => x.Uuid != Guid.Empty.ToString()).Select(p => p.NameClean)));
    }
}
