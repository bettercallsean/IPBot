namespace IPBot.Common.Dtos;

public record FlaggedUserDto
{
    public ulong UserId { get; set; }
    public string Username { get; set; }
    public uint FlaggedCount { get; set; }
}
