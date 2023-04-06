using IPBot.Infrastructure.Helpers;
using IPBot.Infrastructure.Interfaces;

namespace IPBot.Commands;

public class IPCommands : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IIPService _ipService;

    public IPCommands(IIPService ipService)
    {
        _ipService = ipService;
    }

#if DEBUG
    [SlashCommand("ip_debug", "get the current IP of the server")]
#else
    [SlashCommand("ip", "get the current IP of the server")]
#endif
    public async Task GetSeverDomainNameAsync()
    {
        var serverDomain = ServerDomainHelper.GetCurrentServerDomain();
        var ip = await _ipService.GetServerIPAsync();

        await RespondAsync($"`{serverDomain}` or `{ip}`");
    }
}