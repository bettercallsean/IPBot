using System.Text.Json.Serialization;

namespace IPBot.Models.TenorModels;

public class Medium
{
    [JsonPropertyName("gif")]
    public Gif Gif { get; set; }

    [JsonPropertyName("mp4")]
    public Mp4 Mp4 { get; set; }

    [JsonPropertyName("tinygif")]
    public Tinygif Tinygif { get; set; }
}