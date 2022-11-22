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

    [HttpGet("GetCurrentIP")]
    public async Task<string> GetCurrentIPAsync()
    {
        return await IPHelper.GetCurrentIPAsync();
    }
}