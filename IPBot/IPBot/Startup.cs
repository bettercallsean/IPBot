using Discord;
using IPBot.Configuration;
using IPBot.Extensions;
using IPBot.Helpers;
using IPBot.Services.API.Authenticator;
using IPBot.Services.Bot;
using RestSharp;
using Serilog;

namespace IPBot;

internal class Startup
{
    public Startup()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory);

        var environment = DebugHelper.IsDebug() ? "Development" : "Production";
        builder.AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables();

        var config = builder.Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom
            .Configuration(config)
            .CreateLogger();
    }

    public static async void Configure()
    {
        var _ = new Startup();
        await ConfigureBotAsync();
    }

    private static async Task ConfigureBotAsync()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);

        var provider = services.BuildServiceProvider();
        await provider.GetRequiredService<CommandHandler>().InitializeAsync();

        await provider.GetRequiredService<StartupService>().StartAsync();

        await Task.Delay(Timeout.Infinite);
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services
            .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
                MessageCacheSize = 1000,
                GatewayIntents = GatewayIntents.All,
                AlwaysDownloadUsers = true
            }))
            .AddSingleton<CommandHandler>()
            .AddLogging(config =>
            {
                config.AddSerilog();
            })
            .RegisterServices()
            .AddSingleton(x => x.GetRequiredService<BotConfiguration>())
            .AddSingleton<JwtAuthenticator>()
            .AddSingleton<IRestClient>(x =>
            {
                var botConfiguration = x.GetRequiredService<BotConfiguration>();
                var authenticator = x.GetRequiredService<JwtAuthenticator>();

                var options = new RestClientOptions(botConfiguration.APIEndpoint)
                {
                    Authenticator = authenticator
                };

                return new RestClient(options);
            })
            .AddHttpClient();
    }
}
