using Discord;

namespace IPBot.Models.FixUpXModels;

public class Mosaic
{
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("formats")]
    public Formats Formats { get; set; }
}