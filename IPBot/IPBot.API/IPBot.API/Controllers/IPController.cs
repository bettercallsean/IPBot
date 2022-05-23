using IPBot.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IPBot.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IPController : ControllerBase, IIPService
{
    private static readonly string IPFilePath = Path.Combine(AppContext.BaseDirectory, @"../current_domain.txt");
    
    [HttpGet("GetCurrentServerDomain")]
    public async Task<string> GetCurrentDomainAsync()
    {
        var serverDomain = await System.IO.File.ReadAllTextAsync(IPFilePath);
        serverDomain = serverDomain.Trim();
        
        return $"{serverDomain} \n \n" + 
               $"New feature! You can now connect to Minecraft, Ark etc. etc. using {serverDomain} instead of using the IP 😄.";
    }
}