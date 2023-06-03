﻿using IPBot.Shared.Services;
using Microsoft.AspNetCore.Authorization;

namespace IPBot.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]/[action]")]
public class AnimeAnalyserController : ControllerBase
{
    private readonly IAnimeAnalyserService _animeAnalyserService;
    
    public AnimeAnalyserController(IAnimeAnalyserService animeAnalyserService)
    {
        _animeAnalyserService = animeAnalyserService;
    }
    
    [HttpGet("{imageUrl}")]
    public async Task<double> GetAnimeScoreAsync(string imageUrl)
    {
        return await _animeAnalyserService.GetAnimeScoreAsync(imageUrl);
    }
}
