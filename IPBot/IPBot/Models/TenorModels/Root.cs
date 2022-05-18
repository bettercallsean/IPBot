namespace IPBot.Models.TenorModels;

public class Root
{
    [JsonProperty("results")]
    public List<Result> Results { get; set; }
}