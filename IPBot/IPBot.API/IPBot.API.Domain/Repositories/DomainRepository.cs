using IPBot.API.Domain.Data;
using IPBot.API.Domain.Interfaces;

namespace IPBot.API.Domain.Repositories;

public class DomainRepository : BaseRepository<Entities.Domain>, IDomainRepository
{
    public DomainRepository(IIPBotDataContext ipBotDataContext) : base(ipBotDataContext) { }
}