using System.Text.Json.Serialization;

namespace IPBot.Models
{
    internal class ServerInfo
    {
        [JsonPropertyName("online")]
        public bool Online { get; set; } = false;

        [JsonPropertyName("map")]
        public string Map { get; set; }

        [JsonPropertyName("player_count")]
        public int PlayerCount { get; set; }

        [JsonPropertyName("player_names")]
        public List<string> PlayerNames { get; set; }
    }
}