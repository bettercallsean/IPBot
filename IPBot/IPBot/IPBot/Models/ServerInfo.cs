namespace IPBot.Models;

internal record ServerInfo
{
    [JsonProperty("online")]
    public bool Online { get; set; }

    [JsonProperty("map")]
    public string Map { get; set; }

    [JsonProperty("player_count")]
    public int PlayerCount { get; set; }

    [JsonProperty("player_names")]
    public List<string> PlayerNames { get; set; }
}