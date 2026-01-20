using System.Text.Json.Serialization;

namespace IPBot.Models.TenorModels;

public class Tinygif
{
    [JsonPropertyName("dims")]
    public List<int> Dims { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("preview")]
    public string Preview { get; set; }

    [JsonPropertyName("size")]
    public int Size { get; set; }
}