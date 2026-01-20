using System.Text.Json.Serialization;

namespace IPBot.Models.FixUpXModels;

public class Tweet
{
    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("text")]
    public string Text { get; set; }

    [JsonPropertyName("raw_text")]
    public RawText RawText { get; set; }

    [JsonPropertyName("author")]
    public Author Author { get; set; }

    [JsonPropertyName("replies")]
    public int Replies { get; set; }

    [JsonPropertyName("retweets")]
    public int Retweets { get; set; }

    [JsonPropertyName("likes")]
    public int Likes { get; set; }

    [JsonPropertyName("created_at")]
    public string CreatedAt { get; set; }

    [JsonPropertyName("created_timestamp")]
    public int CreatedTimestamp { get; set; }

    [JsonPropertyName("possibly_sensitive")]
    public bool PossiblySensitive { get; set; }

    [JsonPropertyName("views")]
    public int Views { get; set; }

    [JsonPropertyName("is_note_tweet")]
    public bool IsNoteTweet { get; set; }

    [JsonPropertyName("community_note")]
    public object CommunityNote { get; set; }

    [JsonPropertyName("lang")]
    public string Lang { get; set; }

    [JsonPropertyName("replying_to")]
    public object ReplyingTo { get; set; }

    [JsonPropertyName("replying_to_status")]
    public object ReplyingToStatus { get; set; }

    [JsonPropertyName("media")]
    public Media Media { get; set; }

    [JsonPropertyName("source")]
    public string Source { get; set; }

    [JsonPropertyName("twitter_card")]
    public string TwitterCard { get; set; }

    [JsonPropertyName("color")]
    public object Color { get; set; }

    [JsonPropertyName("provider")]
    public string Provider { get; set; }

    [JsonPropertyName("quote")]
    public Quote Quote { get; set; }
}