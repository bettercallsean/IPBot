using IPBot.API.Repositories.Data;
using IPBot.API.Repositories.Interfaces.Repositories;
using IPBot.API.Repositories.Models;

namespace IPBot.API.Repositories.Repositories;

public class DomainRepository : BaseRepository<Domain>, IDomainDataService
{
    public DomainRepository(IIPBotDataContext ipBotDataContext) : base(ipBotDataContext) { }
}