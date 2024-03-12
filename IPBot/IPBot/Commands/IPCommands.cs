using IPBot.Common.Services;
using Microsoft.Extensions.Logging;

namespace IPBot.Commands;

public class IPCommands(ILogger<IPCommands> logger, IIPService ipService) : InteractionModuleBase<SocketInteractionContext>
{
#if DEBUG
    [SlashCommand("ip_debug", "get the current IP of the server")]
#else
    [SlashCommand("ip", "get the current IP of the server")]
#endif
    public async Task GetSeverDomainNameAsync()
    {
        logger.LogInformation("GetSeverDomainNameAsync executed");

        var serverDomain = await ipService.GetCurrentServerDomainAsync();
        var ip = await ipService.GetServerIPAsync();

        await RespondAsync($"`{serverDomain}` or `{ip}`");
    }
}