namespace IPBot.DTOs.Dtos;

public class ServerInfoDto
{
    public string ServerName { get; set; }
    public bool Online { get; set; }
    public string Map { get; set; }
    public int PlayerCount { get; set; }
    public List<string> PlayerNames { get; set; }
}

