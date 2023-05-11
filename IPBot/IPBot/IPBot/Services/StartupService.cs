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
    private readonly IDiscordService _discordService;
    private readonly HubConnection _hubConnection;

    private string _serverDomain;

    public StartupService(ILogger<StartupService> logger, IConfiguration configuration, IIPService ipService, 
        DiscordSocketClient discord, InteractionService commands, IDiscordService discordService)
    {
        _logger = logger;
        _configuration = configuration;
        _ipService = ipService;
        _discord = discord;
        _commands = commands;
        _discordService = discordService;
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
        
        _discord.Ready += DiscordOnReadyAsync;
        _discord.Connected += DiscordOnConnectedAsync;
        _discord.Disconnected += DiscordOnDisconnectedAsync;
    }

    private Task DiscordOnDisconnectedAsync(Exception arg)
    {
        _logger.LogInformation("Disconnected");
        
        return Task.CompletedTask;
    }

    private async Task DiscordOnConnectedAsync()
    {
        _logger.LogInformation("Connected");
        
        if(string.IsNullOrEmpty(_serverDomain))
            _serverDomain = await _ipService.GetCurrentServerDomainAsync();

        await _discord.SetGameAsync(_serverDomain);
    }

    private async Task DiscordOnReadyAsync()
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
        var discordChannels = await _discordService.GetInUseDiscordChannelsAsync();
        
        _logger.LogInformation("Server IP updated to {IP}", ip);
        foreach (var channel in discordChannels)
        {
            var guild = _discord.GetGuild(channel.GuildId);
            var textChannel = guild.GetTextChannel(channel.Id);
            
            _logger.LogInformation("Sending IP update message to {GuildName} - {ChannelName}", guild.Name, textChannel.Name);
            await textChannel.SendMessageAsync($"⚠️ Beep boop. The server IP has changed to `{ip}` ⚠️");
        }
    }
}
