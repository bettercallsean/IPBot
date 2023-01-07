using IPBot.DataServices.Interfaces;

namespace IPBot.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IPController : ControllerBase, IIPService
{
    private readonly IUserDataService _userDataService;

    public IPController(IUserDataService userDataService)
    {
        _userDataService = userDataService;
    }

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