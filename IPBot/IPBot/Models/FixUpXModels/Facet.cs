namespace IPBot.Models.FixUpXModels;

public class Facet
{
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("indices")]
    public List<int> Indices { get; set; }

    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("display")]
    public string Display { get; set; }

    [JsonProperty("original")]
    public string Original { get; set; }

    [JsonProperty("replacement")]
    public string Replacement { get; set; }
}