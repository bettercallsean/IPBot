using System.ComponentModel.DataAnnotations;

namespace IPBot.API.Domain.Entities;

public class FlaggedUser
{
    [Key]
    public ulong UserId { get; set; }
    public string Username { get; set; }
    public uint FlaggedCount { get; set; }
}
