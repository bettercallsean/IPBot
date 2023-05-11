using IPBot.API.Repositories.Data;
using IPBot.API.Repositories.Interfaces;
using IPBot.API.Repositories.Models;

namespace IPBot.API.Repositories.Repositories;

public class DomainRepository : BaseRepository<Domain>, IDomainRepository
{
    public DomainRepository(IIPBotDataContext ipBotDataContext) : base(ipBotDataContext) { }
}