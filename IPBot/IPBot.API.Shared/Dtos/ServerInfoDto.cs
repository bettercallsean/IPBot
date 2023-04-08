using System.Text.Json.Serialization;

namespace IPBot.API.Shared.Dtos;

public record ServerInfoDto
{
    [JsonPropertyName("server_name")]
    public string ServerName { get; set; }
    [JsonPropertyName("online")]
    public bool Online { get; set; }
    [JsonPropertyName("map")]
    public string Map { get; set; }
    [JsonPropertyName("player_count")]
    public int PlayerCount { get; set; }
    [JsonPropertyName("player_names")]
    public List<string> PlayerNames { get; set; }
}

