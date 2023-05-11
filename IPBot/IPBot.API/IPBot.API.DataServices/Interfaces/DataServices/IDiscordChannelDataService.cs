using IPBot.API.DataServices.Models;

namespace IPBot.API.DataServices.Interfaces.DataServices;

public interface IDiscordChannelDataService
{
    Task<List<DiscordChannel>> GetAllAsync();
}