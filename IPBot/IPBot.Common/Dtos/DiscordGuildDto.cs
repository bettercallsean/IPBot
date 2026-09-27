namespace IPBot.Common.Dtos;

public record DiscordGuildDto
{
    public ulong Id { get; set; }
    public string Name { get; set; }
    public bool CheckForTwitterLinks { get; set; }
}
