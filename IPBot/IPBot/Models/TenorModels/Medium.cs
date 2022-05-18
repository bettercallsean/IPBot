namespace IPBot.Models.TenorModels;

public class Medium
{
    [JsonProperty("gif")]
    public Gif Gif { get; set; }

    [JsonProperty("mp4")]
    public Mp4 Mp4 { get; set; }

    [JsonProperty("tinygif")]
    public Tinygif Tinygif { get; set; }
}