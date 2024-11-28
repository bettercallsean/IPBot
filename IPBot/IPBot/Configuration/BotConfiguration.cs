namespace IPBot.Configuration;

public class BotConfiguration
{
    public string APIEndpoint { get; set; }
    public string TenorAPIKey { get; set; }
    public string BotToken { get; set; }
    public string TestGuild { get; set; }
    public APILogin APILogin { get; set; }
}

public class APILogin
{
    public string Username { get; set; }
    public string Password { get; set; }
}
