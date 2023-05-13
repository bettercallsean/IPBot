namespace IPBot.API.Domain.Entities;

public record Domain
{
    public int Id { get; set; }
    public string Description { get; set; }
    public string URL { get; set; }
}