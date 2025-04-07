using IPBot.Common.Services;

namespace IPBot.Commands;

public class IPCommands : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ILogger<IPCommands> _logger;
    private readonly IIPService _ipService;
    public IPCommands(ILogger<IPCommands> logger, IIPService ipService)
    {
        _logger = logger;
        _ipService = ipService;
    }
#if DEBUG
    [SlashCommand("ip_debug", "get the current IP of the server")]
#else
    [SlashCommand("ip", "get the current IP of the server")]
#endif
    public async Task GetSeverDomainNameAsync()
    {
        _logger.LogInformation("GetSeverDomainNameAsync executed");

        var serverDomain = await _ipService.GetCurrentServerDomainAsync();
        var ip = await _ipService.GetServerIPAsync();

        await RespondAsync($"`{serverDomain}` or `{ip}`");
    }
}