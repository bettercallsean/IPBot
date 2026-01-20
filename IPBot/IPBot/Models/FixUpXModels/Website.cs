using System.Text.Json.Serialization;

namespace IPBot.Models.FixUpXModels;

public class Website
{
    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("display_url")]
    public string DisplayUrl { get; set; }
}