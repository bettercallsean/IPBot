using IPBot.Infrastructure.Helpers;
using IPBot.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IPBot.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IPController : ControllerBase, IIPService
{
    
    [HttpGet("GetCurrentServerDomain")]
    public async Task<string> GetCurrentDomainAsync()
    {
        var serverDomain = await ServerDomainHelper.GetCurrentServerDomainAsync();
        
        return $"{serverDomain} \n \n" + 
               $"New feature! You can now connect to Minecraft, Ark etc. etc. using {serverDomain} instead of using the IP 😄.";
    }
}