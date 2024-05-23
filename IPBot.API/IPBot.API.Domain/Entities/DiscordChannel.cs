using System.ComponentModel.DataAnnotations.Schema;

namespace IPBot.API.Domain.Entities;

public class DiscordChannel
{
    public ulong Id { get; set; }

    [ForeignKey("DiscordGuild")]
    public ulong GuildId { get; set; }
    public bool UseForBotMessages { get; set; }
    public string Name { get; set; }
    public bool AnalyseForAnime { get; set; }

    public virtual DiscordGuild DiscordGuild { get; set; }
}