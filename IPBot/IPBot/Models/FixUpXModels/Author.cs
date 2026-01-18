namespace IPBot.Models.FixUpXModels;

public class Author
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("screen_name")]
    public string ScreenName { get; set; }

    [JsonProperty("avatar_url")]
    public string AvatarUrl { get; set; }

    [JsonProperty("banner_url")]
    public string BannerUrl { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("location")]
    public string Location { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("followers")]
    public int Followers { get; set; }

    [JsonProperty("following")]
    public int Following { get; set; }

    [JsonProperty("joined")]
    public string Joined { get; set; }

    [JsonProperty("likes")]
    public int Likes { get; set; }

    [JsonProperty("media_count")]
    public int MediaCount { get; set; }

    [JsonProperty("protected")]
    public bool Protected { get; set; }

    [JsonProperty("website")]
    public Website Website { get; set; }

    [JsonProperty("tweets")]
    public int Tweets { get; set; }

    [JsonProperty("avatar_color")]
    public object AvatarColor { get; set; }
}