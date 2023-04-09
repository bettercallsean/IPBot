namespace IPBot.Shared.Dtos;

public record GameServerDto
{
	public int Id { get; set; }
	public int Port { get; set; }
	public bool Active { get; set; }
	public string? Map { get; set; } 
	public int GameId { get; set; }
}