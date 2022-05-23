using IPBot.Infrastructure.Interfaces;

namespace IPBot.Commands;

public class IPCommands : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IIPService _ipService;

    public IPCommands(IIPService ipService)
    {
        _ipService = ipService;
    }
    
    [SlashCommand("ip", "get the current IP of the server")]
    public async Task GetIPAsync()
    {
        var ip = await _ipService.GetCurrentDomainAsync();
        await RespondAsync(ip);
    }
}