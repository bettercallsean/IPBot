namespace IPBot.Models.TenorModels;

public class Tinygif
{
    [JsonProperty("dims")]
    public List<int> Dims { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("preview")]
    public string Preview { get; set; }

    [JsonProperty("size")]
    public int Size { get; set; }
}