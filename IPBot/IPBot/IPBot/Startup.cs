using IPBot.DataServices;
using IPBot.Helpers;
using IPBot.Infrastructure.Interfaces;
using IPBot.Services;
using Microsoft.Extensions.DependencyInjection;

namespace IPBot;

public class Startup
{
    private readonly IConfigurationRoot _config;

    public Startup()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory);

        builder.AddJsonFile(DebugHelper.IsDebug()
            ? $"{Constants.ConfigDirectory}/appsettings.Dev.json"
            : $"{Constants.ConfigDirectory}/appsettings.json");

        _config = builder.Build();
    }

    public static async Task RunAsync()
    {
        var startup = new Startup();
        await startup.ConfigureBotAsync();
    }

    private async Task ConfigureBotAsync()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);

        var provider = services.BuildServiceProvider();
        await provider.GetRequiredService<CommandHandler>().InitializeAsync();

        await provider.GetRequiredService<StartupService>().StartAsync();

        await Task.Delay(Timeout.Infinite);
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services
            .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = Discord.LogSeverity.Verbose,
                MessageCacheSize = 1000
            }))
            .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
            .AddSingleton<CommandHandler>()
            .AddScoped<StartupService>()
            .AddScoped<MessageAnalyserService>()
            .AddScoped<TenorApiHelper>()
            .AddSingleton<IGameServerService, GameServerDataService>()
            .AddSingleton<IIPService, IPDataService>()
            .AddSingleton<IAnimeAnalyser, AnimeAnalyserDataService>()
            .AddSingleton(_config)
            .AddHttpClient(string.Empty, x =>
            {
                x.BaseAddress = new Uri(_config["apiEndpoint"]);
            });
    }
}
