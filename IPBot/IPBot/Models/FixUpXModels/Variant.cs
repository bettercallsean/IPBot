namespace IPBot.Models.FixUpXModels;

public class Variant
{
    [JsonProperty("content_type")]
    public string ContentType { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("bitrate")]
    public int? Bitrate { get; set; }
}