namespace IPBot.Models.TenorModels;

public class Mp4
{
    [JsonProperty("preview")]
    public string Preview { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("duration")]
    public double Duration { get; set; }

    [JsonProperty("size")]
    public int Size { get; set; }

    [JsonProperty("dims")]
    public List<int> Dims { get; set; }
}