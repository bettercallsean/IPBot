using IPBot.Common.Services;
using IPBot.Helpers;
using Microsoft.AspNetCore.SignalR.Client;

namespace IPBot.Services.Bot;

public class StartupService(ILogger<StartupService> logger, IConfiguration configuration, IIPService ipService, DiscordSocketClient discord, InteractionService commands, IDiscordService discordService, MessageAnalyserService messageAnalyserService)
{
    private readonly HubConnection _hubConnection = new HubConnectionBuilder()
            .WithUrl($"{configuration["APIEndpoint"]}/hubs/iphub")
            .WithAutomaticReconnect()
            .Build();

    private string _serverDomain;

    public async Task StartAsync()
    {
        logger.LogInformation("Starting...");

        var token = configuration["BotToken"];

        await discord.LoginAsync(Discord.TokenType.Bot, token);
        await discord.StartAsync();

        _hubConnection.On("UpdateIP", async (string ip) =>
            { await PostUpdatedIPAsync(ip); });

        await _hubConnection.StartAsync();

        discord.Ready += DiscordOnReadyAsync;
        discord.Connected += DiscordOnConnectedAsync;
        discord.Disconnected += DiscordOnDisconnectedAsync;
        discord.MessageReceived += OnMessageRecivedAsync;
    }

    private async Task OnMessageRecivedAsync(SocketMessage arg) => await messageAnalyserService.CheckMessageForAnimeAsync(arg);

    private Task DiscordOnDisconnectedAsync(Exception arg)
    {
        logger.LogInformation("Disconnected");

        return Task.CompletedTask;
    }

    private async Task DiscordOnConnectedAsync()
    {
        logger.LogInformation("Connected");

        if (string.IsNullOrEmpty(_serverDomain))
            _serverDomain = await ipService.GetCurrentServerDomainAsync();

        await discord.SetGameAsync(_serverDomain);
    }

    private async Task DiscordOnReadyAsync()
    {
        if (DebugHelper.IsDebug())
        {
            var guildId = ulong.Parse(configuration["TestGuild"]);
            await commands.RegisterCommandsToGuildAsync(guildId);
        }
        else
            await commands.RegisterCommandsGloballyAsync();
    }

    private async Task PostUpdatedIPAsync(string ip)
    {
        var discordChannels = await discordService.GetInUseDiscordChannelsAsync();

        logger.LogInformation("Server IP updated to {IP}", ip);
        var ipUpdatedMessage = $"⚠️ Beep boop. The server IP has changed to `{ip}` ⚠️";

        foreach (var channel in discordChannels)
        {
            var guild = discord.GetGuild(channel.GuildId);
            var textChannel = guild.GetTextChannel(channel.Id);

            logger.LogInformation("Sending IP update message to {GuildName} - {ChannelName}", guild.Name, textChannel.Name);
            await textChannel.SendMessageAsync(ipUpdatedMessage);
        }
    }
}
