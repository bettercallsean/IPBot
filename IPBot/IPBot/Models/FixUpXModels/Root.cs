using System.Text.Json.Serialization;

namespace IPBot.Models.FixUpXModels;

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

public class Root
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("tweet")]
    public Tweet Tweet { get; set; }
}