using System.Text.Json.Serialization;

namespace IPBot.Models.FixUpXModels;

public class Author
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("screen_name")]
    public string Username { get; set; }

    [JsonPropertyName("avatar_url")]
    public string AvatarUrl { get; set; }

    [JsonPropertyName("banner_url")]
    public string BannerUrl { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("location")]
    public string Location { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("followers")]
    public int Followers { get; set; }

    [JsonPropertyName("following")]
    public int Following { get; set; }

    [JsonPropertyName("joined")]
    public string Joined { get; set; }

    [JsonPropertyName("likes")]
    public int Likes { get; set; }

    [JsonPropertyName("media_count")]
    public int MediaCount { get; set; }

    [JsonPropertyName("protected")]
    public bool Protected { get; set; }

    [JsonPropertyName("website")]
    public Website Website { get; set; }

    [JsonPropertyName("tweets")]
    public int Tweets { get; set; }

    [JsonPropertyName("avatar_color")]
    public object AvatarColor { get; set; }
}