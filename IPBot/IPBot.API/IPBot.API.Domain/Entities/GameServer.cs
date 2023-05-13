namespace IPBot.API.Domain.Entities;

public record GameServer
{
	public int Id { get; set; }
	public int Port { get; set; }
	public bool Active { get; set; }
	public string? Map { get; set; } 
	public int GameId { get; set; }

	public virtual Game Game { get; set; }
}
