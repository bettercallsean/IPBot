namespace IPBot.API.Domain.Entities;

public record DiscordChannel
{
    public ulong Id { get; set; }
    public ulong GuildId { get; set; }
    public bool InUse { get; set; }
    public string ChannelDescription { get; set; }
}