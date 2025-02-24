namespace IPBot.API.Domain.Entities;

public class DiscordGuild
{
    public ulong Id { get; set; }
    public string Name { get; set; }

    public virtual IEnumerable<DiscordChannel> DiscordChannels { get; set; }
}
