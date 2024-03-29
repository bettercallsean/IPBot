﻿namespace IPBot.API.Domain.Entities;

public class Game
{
	public int Id { get; set; }
	public string Name { get; set; }
	public string ShortName { get; set; }

	public virtual IEnumerable<GameServer> GameServers { get; set; }
}
