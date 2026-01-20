using System.Text.Json.Serialization;

namespace IPBot.Models.FixUpXModels;

public class Media
{
    [JsonPropertyName("all")]
    public List<All> All { get; set; }

    [JsonPropertyName("videos")]
    public List<Video> Videos { get; set; }

    [JsonPropertyName("mosaic")]
    public Mosaic Mosaic { get; set; }
}