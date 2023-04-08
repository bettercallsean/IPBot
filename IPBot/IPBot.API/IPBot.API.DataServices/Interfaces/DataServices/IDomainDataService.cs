using IPBot.DataServices.Models;

namespace IPBot.DataServices.Interfaces.DataServices;

public interface IDomainDataService
{
    Task<Domain> GetByIdAsync(int id);
    Task<Domain> GetByDescriptionAsync(string description);
}