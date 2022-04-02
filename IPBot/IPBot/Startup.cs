using IPBot.Services;
using Microsoft.Extensions.DependencyInjection;

namespace IPBot;

public class Startup
{
    public IConfigurationRoot Configuration { get; }

    public Startup(string[] args)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile($"{Constants.ConfigDirectory}/config.json");

        Configuration = builder.Build();
    }

    public static async Task RunAsync(string[] args)
    {
        var startup = new Startup(args);
        await startup.RunAsync();
    }

    private async Task RunAsync()
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
            .AddSingleton(Configuration);
    }
}
