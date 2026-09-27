namespace IPBot.Configuration;

public record BotConfiguration
{
    public required string APIEndpoint { get; set; }
    public required string TenorAPIKey { get; set; }
    public required string BotToken { get; set; }
    public required string TestGuild { get; set; }
    public required APILogin APILogin { get; set; }
}

public record APILogin
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}
