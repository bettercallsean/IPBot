using Discord;
using IPBot.Common.Services;
using IPBot.Configuration;
using IPBot.Helpers;
using IPBot.Interfaces;
using IPBot.Services.API;
using IPBot.Services.Bot;
using RestSharp;
using Serilog;

namespace IPBot;

public class Startup
{
    private readonly IConfigurationRoot _config;

    public Startup()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory);

        var environment = DebugHelper.IsDebug() ? "Development" : "Production";
        builder.AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{environment}.json", optional: true);

        _config = builder.Build();

        Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(_config).CreateLogger();
    }

    public static async void Configure()
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
                LogLevel = LogSeverity.Verbose,
                MessageCacheSize = 1000,
                GatewayIntents = GatewayIntents.All,
                AlwaysDownloadUsers = true
            }))
            .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
            .AddSingleton<CommandHandler>()
            .AddScoped<StartupService>()
            .AddScoped<MessageAnalyserService>()
            .AddSingleton<IGameService, GameService>()
            .AddSingleton<IIPService, IPService>()
            .AddSingleton<IAnimeAnalyserService, AnimeAnalyserService>()
            .AddSingleton<IDiscordService, DiscordService>()
            .AddSingleton<ITenorApiHelper, TenorApiHelper>()
            .AddSingleton(_config.Get<BotConfiguration>())
            .AddLogging(config =>
            {
                config.AddSerilog();
            })
            .AddSingleton<IRestClient>(new RestClient(new HttpClient
            {
                BaseAddress = new Uri(_config.GetValue<string>("APIEndpoint") ?? throw new Exception("APIEndpoint is empty"))
            }))
            .AddHttpClient();
    }
}
