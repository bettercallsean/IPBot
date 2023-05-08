using IPBot.Helpers;
using IPBot.Shared.Services;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace IPBot.Services;

public class StartupService
{
    private readonly ILogger<StartupService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IIPService _ipService;
    private readonly DiscordSocketClient _discord;
    private readonly InteractionService _commands;
    private readonly HubConnection _hubConnection;

    public StartupService(ILogger<StartupService> logger, IConfiguration configuration, IIPService ipService, DiscordSocketClient discord, InteractionService commands)
    {
        _logger = logger;
        _configuration = configuration;
        _ipService = ipService;
        _discord = discord;
        _commands = commands;
        _hubConnection = new HubConnectionBuilder()
            .WithUrl($"{configuration["APIEndpoint"]}/hubs/iphub")
            .WithAutomaticReconnect()
            .Build();
    }

    public async Task StartAsync()
    {
        _logger.LogInformation("Starting...");

        var token = _configuration["BotToken"];

        await _discord.LoginAsync(Discord.TokenType.Bot, token);
        await _discord.StartAsync();

        _hubConnection.On("UpdateIP", async (string ip) => 
            { await PostUpdatedIPAsync(ip); });
        
        await _hubConnection.StartAsync();
        
        _discord.Ready += ReadyAsync;
        _discord.Connected += DiscordOnConnected;
    }

    private async Task DiscordOnConnected()
    {
        _logger.LogInformation("Connected");

        var serverDomain = await _ipService.GetCurrentServerDomainAsync();

        await _discord.SetGameAsync(serverDomain);
    }

    private async Task ReadyAsync()
    {
        if (DebugHelper.IsDebug())
        {
            var guildId = ulong.Parse(_configuration["TestGuild"]);
            await _commands.RegisterCommandsToGuildAsync(guildId);
        }
        else
        {
            await _commands.RegisterCommandsGloballyAsync();
        }
    }

    private async Task PostUpdatedIPAsync(string ip)
    {
        foreach (var foo in _configuration.GetSection("DiscordServers").GetChildren())
        {
            var guild = _discord.GetGuild(ulong.Parse(foo.Key));
            var channel = guild.GetTextChannel(ulong.Parse(foo.Value));
            
            await channel.SendMessageAsync($"⚠️ Beep boop. The server IP has changed to `{ip}` ⚠️");
        }
    }
}
