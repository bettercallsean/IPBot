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
        discord.Disconnected += DiscordOnDisconnectedAsync;
        discord.MessageReceived += OnMessageReceivedAsync;
    }

    private async Task OnMessageReceivedAsync(SocketMessage arg)
    {
        if (arg.Author.IsBot) return;

        await messageAnalyserService.CheckMessageForAnimeAsync(arg);
        await messageAnalyserService.CheckMessageForHatefulContentAsync(arg);
    }

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
