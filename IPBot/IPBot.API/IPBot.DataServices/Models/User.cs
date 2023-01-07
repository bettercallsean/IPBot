using System.ComponentModel.DataAnnotations;

namespace IPBot.DataServices.Models;

public class User
{
    [Key]
    public int Id { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string PasswordSalt { get; set; }
}
