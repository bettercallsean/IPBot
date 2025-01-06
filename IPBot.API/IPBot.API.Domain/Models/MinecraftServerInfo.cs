using System.Text.Json.Serialization;

namespace IPBot.API.Domain.Models;

public class MinecraftServerInfo
{
    public bool Online { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }
    public Players Players { get; set; }
    public Motd Motd { get; set; }
}

public class Players
{
    public int Online { get; set; }
    public List[] List { get; set; }
}

public class List
{
    public string Uuid { get; set; }
    [JsonPropertyName("name_raw")]
    public string NameRaw { get; set; }
    [JsonPropertyName("name_clean")]
    public string NameClean { get; set; }
    [JsonPropertyName("name_html")]
    public string NameHtml { get; set; }
}

public class Motd
{
    public string Raw { get; set; }
    public string Clean { get; set; }
    public string Html { get; set; }
}
