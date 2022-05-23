﻿namespace IPBot.Infrastructure.Interfaces;

public interface IIPService
{
    Task<string> GetCurrentDomainAsync();
    Task<string> GetCurrentIPAsync();
}