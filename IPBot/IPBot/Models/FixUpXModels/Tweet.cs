namespace IPBot.Models.FixUpXModels;

public class Tweet
{
    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("text")]
    public string Text { get; set; }

    [JsonProperty("raw_text")]
    public RawText RawText { get; set; }

    [JsonProperty("author")]
    public Author Author { get; set; }

    [JsonProperty("replies")]
    public int Replies { get; set; }

    [JsonProperty("retweets")]
    public int Retweets { get; set; }

    [JsonProperty("likes")]
    public int Likes { get; set; }

    [JsonProperty("created_at")]
    public string CreatedAt { get; set; }

    [JsonProperty("created_timestamp")]
    public int CreatedTimestamp { get; set; }

    [JsonProperty("possibly_sensitive")]
    public bool PossiblySensitive { get; set; }

    [JsonProperty("views")]
    public int Views { get; set; }

    [JsonProperty("is_note_tweet")]
    public bool IsNoteTweet { get; set; }

    [JsonProperty("community_note")]
    public object CommunityNote { get; set; }

    [JsonProperty("lang")]
    public string Lang { get; set; }

    [JsonProperty("replying_to")]
    public object ReplyingTo { get; set; }

    [JsonProperty("replying_to_status")]
    public object ReplyingToStatus { get; set; }

    [JsonProperty("media")]
    public Media Media { get; set; }

    [JsonProperty("source")]
    public string Source { get; set; }

    [JsonProperty("twitter_card")]
    public string TwitterCard { get; set; }

    [JsonProperty("color")]
    public object Color { get; set; }

    [JsonProperty("provider")]
    public string Provider { get; set; }
}