using System.Text.Json.Serialization;

namespace IPBot.Models.FixUpXModels;

public class Variant
{
    [JsonPropertyName("content_type")]
    public string ContentType { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("bitrate")]
    public int? Bitrate { get; set; }
}