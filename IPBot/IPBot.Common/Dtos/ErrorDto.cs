namespace IPBot.Common.Dtos;

public record ErrorDto
{
    public int StatusCode { get; set; }
    public string ErrorMessage { get; set; }
}