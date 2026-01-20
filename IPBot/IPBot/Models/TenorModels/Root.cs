using System.Text.Json.Serialization;

namespace IPBot.Models.TenorModels;

public class Root
{
    [JsonPropertyName("results")]
    public List<Result> Results { get; set; }
}