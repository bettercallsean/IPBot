using IPBot.API.Domain.Data;
using IPBot.API.Domain.Repositories;

namespace IPBot.API.Domain;

public class DiscordGuildRepository(IIPBotDataContext ipBotDataContext) : BaseRepository<DiscordGuild>(ipBotDataContext), IDiscordGuildRepository
{
}
