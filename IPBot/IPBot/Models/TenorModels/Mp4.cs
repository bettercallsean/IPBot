using System.Text.Json.Serialization;

namespace IPBot.Models.TenorModels;

public class Mp4
{
    [JsonPropertyName("preview")]
    public string Preview { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("duration")]
    public double Duration { get; set; }

    [JsonPropertyName("size")]
    public int Size { get; set; }

    [JsonPropertyName("dims")]
    public List<int> Dims { get; set; }
}