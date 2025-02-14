namespace IPBot.Common.Dtos;

public record CategoryAnalysisDto
{
    public string Category { get; set; }
    public int Severity { get; set; }
}
