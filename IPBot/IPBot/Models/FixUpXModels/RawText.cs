namespace IPBot.Models.FixUpXModels;

public class RawText
{
    [JsonProperty("text")]
    public string Text { get; set; }

    [JsonProperty("facets")]
    public List<Facet> Facets { get; set; }
}