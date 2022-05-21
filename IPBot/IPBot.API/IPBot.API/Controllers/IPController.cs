using IPBot.API.Helpers;
using IPBot.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IPBot.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IPController : ControllerBase, IIPService
{
    [HttpGet("GetCurrentIP")]
    public async Task<string> GetCurrentIPAsync()
    {
        return await IPHelper.GetIPFromFileAsync();
    }
}