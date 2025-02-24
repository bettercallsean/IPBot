namespace IPBot.Common.Dtos;

public record DiscordChannelDto
{
    public ulong Id { get; set; }
    public ulong GuildId { get; set; }
}