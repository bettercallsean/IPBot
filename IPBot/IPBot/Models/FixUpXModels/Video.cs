using System.Text.Json.Serialization;

namespace IPBot.Models.FixUpXModels;

public class Video
{
    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("thumbnail_url")]
    public string ThumbnailUrl { get; set; }

    [JsonPropertyName("duration")]
    public double Duration { get; set; }

    [JsonPropertyName("width")]
    public int Width { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("format")]
    public string Format { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("variants")]
    public List<Variant> Variants { get; set; }
}