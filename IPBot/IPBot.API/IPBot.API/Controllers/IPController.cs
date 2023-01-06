namespace IPBot.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IPController : ControllerBase, IIPService
{
    [HttpGet("GetCurrentServerDomain")]
    public Task<string> GetCurrentDomainAsync()
    {
        return Task.FromResult(ServerDomainHelper.GetCurrentServerDomain());
    }

    [HttpGet("GetLocalIP")]
    public async Task<string> GetLocalIPAsync()
    {
        return await IPHelper.GetLocalIPAsync();
    }

    [HttpGet("GetServerIP")]
    public async Task<string> GetServerIPAsync()
    {
        return await IPHelper.GetSeverIPAsync();
    }
}