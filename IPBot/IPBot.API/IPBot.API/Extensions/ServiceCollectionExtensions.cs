using IPBot.API.AutoMapper;
using IPBot.API.Domain.Interfaces;
using IPBot.API.Domain.Repositories;
using IPBot.API.Services;
using IPBot.Shared.Services;

namespace IPBot.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        RegisterDataServices(services);
        RegisterControllerServices(services);
        RegisterAutoMapperProfiles(services);

        return services;
    }

    private static void RegisterDataServices(IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IGameServerRepository, GameServerRepository>();
        services.AddScoped<IGameRepository, GameRepository>();
        services.AddScoped<IDomainRepository, DomainRepository>();
        services.AddScoped<IDiscordChannelRepository, DiscordChannelRepository>();
    }

    private static void RegisterControllerServices(IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IGameService, GameService>();
        services.AddScoped<IIPService, IPService>();
        services.AddScoped<IDiscordService, DiscordService>();
        services.AddScoped<IAnimeAnalyserService, AnimeAnalyserService>();
    }

    private static void RegisterAutoMapperProfiles(IServiceCollection services)
    {
        services.AddAutoMapper(config =>
        {
            config.AddProfile<GameProfile>();
            config.AddProfile<GameServerProfile>();
            config.AddProfile<DiscordChannelProfile>();
            config.AddProfile<GameServerInfoProfile>();
        });
    }
}
