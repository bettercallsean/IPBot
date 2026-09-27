using Discord;
using IPBot.Configuration;
using IPBot.Extensions;
using IPBot.Helpers;
using IPBot.Services.API.Authoriser;
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
            .AddSingleton<CommandHandler>()
            .RegisterServices()
            .AddSingleton(_config.Get<BotConfiguration>() ?? throw new Exception("BotConfiguration secttion is empty"))
            .AddLogging(config =>
            {
                config.AddSerilog();
            })
            .AddSingleton<IRestClient>(x =>
            {
                var botConfiguration = _config.Get<BotConfiguration>();
                var authenticator = new JwtAuthoriser(botConfiguration.APILogin);
                var options = new RestClientOptions(botConfiguration.APIEndpoint)
                {
                    Authenticator = authenticator
                };

                return new RestClient(options);
            })
            .AddHttpClient();
    }
}
