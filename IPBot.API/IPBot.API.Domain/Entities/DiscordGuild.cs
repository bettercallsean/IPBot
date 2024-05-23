using IPBot.API.Domain.Entities;

namespace IPBot.API.Domain;

public class DiscordGuild
{
    public ulong Id { get; set; }
    public string Name { get; set; }

    public virtual IEnumerable<DiscordChannel> Channels { get; set; }
}
