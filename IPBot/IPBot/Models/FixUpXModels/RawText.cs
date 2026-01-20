using System.Text.Json.Serialization;

namespace IPBot.Models.FixUpXModels;

public class RawText
{
    [JsonPropertyName("text")]
    public string Text { get; set; }

    [JsonPropertyName("facets")]
    public List<Facet> Facets { get; set; }
}