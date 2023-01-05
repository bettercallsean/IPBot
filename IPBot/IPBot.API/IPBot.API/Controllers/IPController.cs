namespace IPBot.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IPController : ControllerBase, IIPService
{
    [HttpGet("GetCurrentServerDomain")]
    public async Task<string> GetCurrentDomainAsync()
    {
        return await ServerDomainHelper.GetCurrentServerDomainAsync();
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