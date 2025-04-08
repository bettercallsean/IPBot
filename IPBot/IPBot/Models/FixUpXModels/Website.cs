namespace IPBot.Models.FixUpXModels;

public class Website
{
    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("display_url")]
    public string DisplayUrl { get; set; }
}