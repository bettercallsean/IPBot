using IPBot.API.DataServices.Data;
using IPBot.API.DataServices.Interfaces.DataServices;
using IPBot.API.DataServices.Models;
using Microsoft.EntityFrameworkCore;

namespace IPBot.API.DataServices.DataServices;

public class DomainDataService : IDomainDataService
{
    private readonly IIPBotDataContext _ipBotDataContext;

    public DomainDataService(IIPBotDataContext ipBotDataContext)
    {
        _ipBotDataContext = ipBotDataContext;
    }

    public async Task<Domain> GetByIdAsync(int id)
    {
        return await _ipBotDataContext.Domains.FindAsync(id);
    }

    public async Task<Domain> GetByDescriptionAsync(string description)
    {
        return await _ipBotDataContext.Domains
            .AsNoTracking()
            .Where(x => x.Description == description)
            .FirstOrDefaultAsync();
    }
}