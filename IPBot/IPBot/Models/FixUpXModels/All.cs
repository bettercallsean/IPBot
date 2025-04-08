namespace IPBot.Models.FixUpXModels;

public class All
{
    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("thumbnail_url")]
    public string ThumbnailUrl { get; set; }

    [JsonProperty("duration")]
    public double Duration { get; set; }

    [JsonProperty("width")]
    public int Width { get; set; }

    [JsonProperty("height")]
    public int Height { get; set; }

    [JsonProperty("format")]
    public string Format { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("variants")]
    public List<Variant> Variants { get; set; }
}