using IPBot.Common.Constants;
using IPBot.Common.Services;
using IPBot.Configuration;
using IPBot.Helpers;
using Microsoft.AspNetCore.SignalR.Client;

namespace IPBot.Services.Bot;

public class StartupService
{
    private readonly HubConnection _hubConnection;

    private string _serverDomain;
    private bool _initialConnection = true;
    private readonly ILogger<StartupService> _logger;
    private readonly BotConfiguration _botConfiguration;
    private readonly IIPService _ipService;
    private readonly DiscordSocketClient _discord;
    private readonly InteractionService _commands;
    private readonly IDiscordService _discordService;
    private readonly MessageAnalyserService _messageAnalyserService;

    public StartupService(ILogger<StartupService> logger, BotConfiguration botConfiguration, IIPService ipService,
        DiscordSocketClient discord, InteractionService commands, IDiscordService discordService,
        MessageAnalyserService messageAnalyserService)
    {
        _logger = logger;
        _botConfiguration = botConfiguration;
        _ipService = ipService;
        _discord = discord;
        _commands = commands;
        _discordService = discordService;
        _messageAnalyserService = messageAnalyserService;
        _hubConnection = new HubConnectionBuilder()
            .WithUrl($"{botConfiguration.APIEndpoint}/hubs/iphub")
            .WithAutomaticReconnect()
            .Build();
    }

    public async Task StartAsync()
    {
        _logger.LogInformation("Starting...");

        await _discord.LoginAsync(Discord.TokenType.Bot, _botConfiguration.BotToken);
        await _discord.StartAsync();

        _hubConnection.On(SignalRHubMethods.UpdateIP, async (string ip) =>
            { await PostUpdatedIPAsync(ip); });

        await _hubConnection.StartAsync();

        _discord.Ready += DiscordOnReadyAsync;
        _discord.Connected += DiscordOnConnectedAsync;
        _discord.MessageReceived += OnMessageReceivedAsync;
    }

    private async Task OnMessageReceivedAsync(SocketMessage arg)
    {
        if (arg.Author.IsBot) return;
        
        await _messageAnalyserService.CheckMessageForAnimeAsync(arg);
        await _messageAnalyserService.CheckMessageForHatefulContentAsync(arg);
        await _messageAnalyserService.CheckForTwitterLinksAsync(arg);
    }

    private async Task DiscordOnConnectedAsync()
    {
        if (_initialConnection)
        {
            _logger.LogInformation("Connected");
            _initialConnection = false;
        }

        if (string.IsNullOrEmpty(_serverDomain))
            _serverDomain = await _ipService.GetCurrentServerDomainAsync();

        await _discord.SetGameAsync(_serverDomain);
    }

    private async Task DiscordOnReadyAsync()
    {
        if (DebugHelper.IsDebug())
        {
            var guildId = ulong.Parse(_botConfiguration.TestGuild);
            await _commands.RegisterCommandsToGuildAsync(guildId);
        }
        else
            await _commands.RegisterCommandsGloballyAsync();
    }

    private async Task PostUpdatedIPAsync(string ip)
    {
        var discordChannels = await _discordService.GetInUseDiscordChannelsAsync();

        _logger.LogInformation("Server IP updated to {IP}", ip);
        var ipUpdatedMessage = $"⚠️ Beep boop. The server IP has changed to `{ip}` ⚠️";

        foreach (var channel in discordChannels)
        {
            var guild = _discord.GetGuild(channel.GuildId);
            var textChannel = guild.GetTextChannel(channel.Id);

            _logger.LogInformation("Sending IP update message to {GuildName} - {ChannelName}", guild.Name, textChannel.Name);
            await textChannel.SendMessageAsync(ipUpdatedMessage);
        }
    }
}
