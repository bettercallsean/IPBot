namespace IPBot.Models.FixUpXModels;

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

public class Root
{
    [JsonProperty("code")]
    public int Code { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }

    [JsonProperty("tweet")]
    public Tweet Tweet { get; set; }
}