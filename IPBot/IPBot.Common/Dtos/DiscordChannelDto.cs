namespace IPBot.Common.Dtos;

public record DiscordChannelDto
{
    public ulong Id { get; set; }
    public string Name { get; set; }
    public ulong GuildId { get; set; }
    public bool UseForBotMessages { get; set; }
    public bool AnalyseForAnime { get; set; }
}