using System.Text.Json.Serialization;

namespace IPBot.Models.FixUpXModels;

public class Formats
{
    [JsonPropertyName("jpeg")]
    public string Jpeg { get; set; }

    [JsonPropertyName("webp")]
    public string Webp { get; set; }
}
