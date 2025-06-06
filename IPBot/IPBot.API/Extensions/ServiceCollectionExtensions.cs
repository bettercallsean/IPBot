﻿using IPBot.API.AutoMapper;
using IPBot.API.Constants;
using IPBot.API.Domain.Interfaces;
using IPBot.API.Domain.Repositories;
using IPBot.API.Services;
using IPBot.Common.Services;

namespace IPBot.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static void RegisterServices(this IServiceCollection services)
    {
        RegisterDataServices(services);
        RegisterControllerServices(services);
        RegisterAutoMapperProfiles(services);
    }

    public static void RegisterHttpClients(this IServiceCollection services)
    {
        services.AddHttpClient(KeyedHttpClientNames.LocalIPClient, c =>
        {
            c.BaseAddress = new("https://api.ipify.org");
        });

        services.AddHttpClient(KeyedHttpClientNames.MinecraftServerClient, c =>
        {
            c.BaseAddress = new("https://api.mcstatus.io/v2");
        });
    }

    private static void RegisterDataServices(IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IGameServerRepository, GameServerRepository>();
        services.AddScoped<IGameRepository, GameRepository>();
        services.AddScoped<IDomainRepository, DomainRepository>();
        services.AddScoped<IDiscordChannelRepository, DiscordChannelRepository>();
        services.AddScoped<IDiscordGuildRepository, DiscordGuildRepository>();
        services.AddScoped<IFlaggedUserRepository, FlaggedUserRepository>();
    }

    private static void RegisterControllerServices(IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IGameService, GameService>();
        services.AddScoped<IIPService, IPService>();
        services.AddScoped<IDiscordService, DiscordService>();
        services.AddScoped<IImageAnalyserService, ImageAnalyserService>();
    }

    private static void RegisterAutoMapperProfiles(IServiceCollection services)
    {
        services.AddAutoMapper(config =>
        {
            config.AddProfile<GameProfile>();
            config.AddProfile<GameServerProfile>();
            config.AddProfile<DiscordChannelProfile>();
            config.AddProfile<GameServerInfoProfile>();
            config.AddProfile<FlaggedUserProfile>();
        });
    }
}
