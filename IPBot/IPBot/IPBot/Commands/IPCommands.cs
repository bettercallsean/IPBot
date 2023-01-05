﻿using IPBot.Infrastructure.Interfaces;

namespace IPBot.Commands;

public class IPCommands : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IIPService _ipService;

    public IPCommands(IIPService ipService)
    {
        _ipService = ipService;
    }

    [SlashCommand("ip", "get the current IP of the server")]
    public async Task GetSeverDomainNameAsync()
    {
        var serverDomain = await _ipService.GetCurrentDomainAsync();
        var ip = await _ipService.GetServerIPAsync();

        await RespondAsync($"`{ip}` {Environment.NewLine}" +
            $"New feature! You can now connect to Minecraft, Ark etc. etc. using `{serverDomain}` instead of using the IP 😄 This shouldn't change so there shouldn't be any worry about having to change it!");
    }
}