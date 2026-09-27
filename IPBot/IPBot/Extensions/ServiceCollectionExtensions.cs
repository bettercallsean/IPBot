using IPBot.Common.Services;
using IPBot.Helpers;
using IPBot.Interfaces.Helpers;
using IPBot.Interfaces.Services;
using IPBot.Services.API;
using IPBot.Services.Bot;

namespace IPBot.Extensions;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        RegisterDiscordService(services);
        RegisterDataServices(services);
        RegisterAnalyserServices(services);
        RegisterHelperServices(services);

        return services;
    }

    private static void RegisterDiscordService(IServiceCollection services)
    {
        services.AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()));
        services.AddScoped<StartupService>();
        services.AddScoped<IMessageMediaAnalyserService, MessageMediaAnalyserService>();
    }

    private static void RegisterDataServices(IServiceCollection services)
    {
        services.AddSingleton<IGameService, GameService>();
        services.AddSingleton<IIPService, IPService>();
        services.AddSingleton<IImageAnalyserService, ImageAnalyserService>();
        services.AddSingleton<IDiscordService, DiscordService>();
    }

    private static void RegisterAnalyserServices(IServiceCollection services)
    {
        services.AddSingleton<ITweetAnalyserService, TweetAnalyserService>();
        services.AddSingleton<IAnimeAnalyserService, AnimeAnalyserService>();
        services.AddSingleton<IHatefulContentAnalyserService, HatefulContentAnalyserService>();
    }

    private static void RegisterHelperServices(IServiceCollection services)
    {
        services.AddSingleton<ITenorApiHelper, TenorApiHelper>();
    }
}
