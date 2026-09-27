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
    private readonly IConfigurationRoot _config;

    public Startup()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory);

        var environment = DebugHelper.IsDebug() ? "Development" : "Production";
        builder.AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables();

        _config = builder.Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom
            .Configuration(_config)
            .CreateLogger();
    }

    public async void Configure()
    {
        var _ = new Startup();
        await ConfigureBotAsync();
    }

    private async Task ConfigureBotAsync()
    {
        var services = ConfigureServices();

        var provider = services.BuildServiceProvider();
        await provider.GetRequiredService<CommandHandler>().InitializeAsync();
        await provider.GetRequiredService<StartupService>().StartAsync();

        await Task.Delay(Timeout.Infinite);
    }

    private ServiceCollection ConfigureServices()
    {
        var services = new ServiceCollection();

        services
            .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
                MessageCacheSize = 1000,
                GatewayIntents = GatewayIntents.All,
                AlwaysDownloadUsers = true
            }))
            .AddLogging(config =>
            {
                config.AddSerilog();
            })
            .RegisterServices()
            .AddSingleton<JwtAuthenticator>()
            .AddSingleton(x => _config.GetRequiredSection("BotConfiguration").Get<BotConfiguration>())
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

        return services;
    }
}
