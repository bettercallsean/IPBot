namespace IPBot.Models.TenorModels;

public class Result
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("content_description")]
    public string ContentDescription { get; set; }

    [JsonProperty("content_rating")]
    public string ContentRating { get; set; }

    [JsonProperty("h1_title")]
    public string H1Title { get; set; }

    [JsonProperty("media")]
    public List<Medium> Media { get; set; }

    [JsonProperty("bg_color")]
    public string BgColor { get; set; }

    [JsonProperty("created")]
    public double Created { get; set; }

    [JsonProperty("itemurl")]
    public string Itemurl { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("tags")]
    public List<object> Tags { get; set; }

    [JsonProperty("flags")]
    public List<object> Flags { get; set; }

    [JsonProperty("shares")]
    public int Shares { get; set; }

    [JsonProperty("hasaudio")]
    public bool Hasaudio { get; set; }

    [JsonProperty("hascaption")]
    public bool Hascaption { get; set; }

    [JsonProperty("source_id")]
    public string SourceId { get; set; }

    [JsonProperty("composite")]
    public object Composite { get; set; }
}