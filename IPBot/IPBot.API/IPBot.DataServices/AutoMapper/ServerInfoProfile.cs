using AutoMapper;
using IPBot.DTOs.Dtos;
using IPBot.Infrastructure.Models;

namespace IPBot.DataServices.AutoMapper;

public class ServerInfoProfile : Profile
{
	public ServerInfoProfile()
	{
		CreateMap<ServerInfo, ServerInfoDto>();
	}
}

