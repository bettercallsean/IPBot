using IPBot.API.Domain.Data;
using IPBot.API.Domain.Interfaces;

namespace IPBot.API.Domain.Repositories;

public class DomainRepository(IIPBotDataContext ipBotDataContext) : BaseRepository<Entities.Domain>(ipBotDataContext), IDomainRepository
{
}