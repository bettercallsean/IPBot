using IPBot.API.DataServices.Models;

namespace IPBot.API.DataServices.Interfaces.DataServices;

public interface IDomainDataService
{
    Task<Domain> GetByIdAsync(int id);
    Task<Domain> GetByDescriptionAsync(string description);
}