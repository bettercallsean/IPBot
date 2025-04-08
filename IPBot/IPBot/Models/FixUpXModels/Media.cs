namespace IPBot.Models.FixUpXModels;

public class Media
{
    [JsonProperty("all")]
    public List<All> All { get; set; }

    [JsonProperty("videos")]
    public List<Video> Videos { get; set; }
}