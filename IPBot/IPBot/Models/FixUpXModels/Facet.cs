using System.Text.Json.Serialization;

namespace IPBot.Models.FixUpXModels;

public class Facet
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("indices")]
    public List<int> Indices { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("display")]
    public string Display { get; set; }

    [JsonPropertyName("original")]
    public string Original { get; set; }

    [JsonPropertyName("replacement")]
    public string Replacement { get; set; }
}