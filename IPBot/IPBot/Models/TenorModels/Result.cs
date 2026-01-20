using System.Text.Json.Serialization;

namespace IPBot.Models.TenorModels;

public class Result
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("content_description")]
    public string ContentDescription { get; set; }

    [JsonPropertyName("content_rating")]
    public string ContentRating { get; set; }

    [JsonPropertyName("h1_title")]
    public string H1Title { get; set; }

    [JsonPropertyName("media")]
    public List<Medium> Media { get; set; }

    [JsonPropertyName("bg_color")]
    public string BgColor { get; set; }

    [JsonPropertyName("created")]
    public double Created { get; set; }

    [JsonPropertyName("itemurl")]
    public string Itemurl { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("tags")]
    public List<object> Tags { get; set; }

    [JsonPropertyName("flags")]
    public List<object> Flags { get; set; }

    [JsonPropertyName("shares")]
    public int Shares { get; set; }

    [JsonPropertyName("hasaudio")]
    public bool Hasaudio { get; set; }

    [JsonPropertyName("hascaption")]
    public bool Hascaption { get; set; }

    [JsonPropertyName("source_id")]
    public string SourceId { get; set; }

    [JsonPropertyName("composite")]
    public object Composite { get; set; }
}