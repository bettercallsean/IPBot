using IPBot.Common.Constants;
using IPBot.Common.Services;
using IPBot.Configuration;
using IPBot.Helpers;
using Microsoft.AspNetCore.SignalR.Client;

namespace IPBot.Services.Bot;

public class StartupService(ILogger<StartupService> logger, BotConfiguration botConfiguration, IIPService ipService,
    DiscordSocketClient discord, InteractionService commands, IDiscordService discordService,
    MessageAnalyserService messageAnalyserService)
{
    private readonly HubConnection _hubConnection = new HubConnectionBuilder()
            .WithUrl($"{botConfiguration.APIEndpoint}/hubs/iphub")
            .WithAutomaticReconnect()
            .Build();

    private string _serverDomain;
    private bool _initialConnection = true;

    public async Task StartAsync()
    {
        logger.LogInformation("Starting...");

        await discord.LoginAsync(Discord.TokenType.Bot, botConfiguration.BotToken);
        await discord.StartAsync();

        _hubConnection.On(SignalRHubMethods.UpdateIP, async (string ip) =>
            { await PostUpdatedIPAsync(ip); });

        await _hubConnection.StartAsync();

        discord.Ready += DiscordOnReadyAsync;
        discord.Connected += DiscordOnConnectedAsync;
        discord.MessageReceived += OnMessageReceivedAsync;
    }

    private async Task OnMessageReceivedAsync(SocketMessage arg)
    {
        if (arg.Author.IsBot) return;

        await messageAnalyserService.CheckMessageForAnimeAsync(arg);
        await messageAnalyserService.CheckMessageForHatefulContentAsync(arg);
    }

    private async Task DiscordOnConnectedAsync()
    {
        if (_initialConnection)
        {
            logger.LogInformation("Connected");
            _initialConnection = false;
        } 
        
        if (string.IsNullOrEmpty(_serverDomain))
            _serverDomain = await ipService.GetCurrentServerDomainAsync();

        await discord.SetGameAsync(_serverDomain);
    }

    private async Task DiscordOnReadyAsync()
    {
        if (DebugHelper.IsDebug())
        {
            var guildId = ulong.Parse(botConfiguration.TestGuild);
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
