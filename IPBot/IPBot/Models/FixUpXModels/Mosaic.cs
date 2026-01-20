using System.Text.Json.Serialization;
using Discord;

namespace IPBot.Models.FixUpXModels;

public class Mosaic
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("formats")]
    public Formats Formats { get; set; }
}