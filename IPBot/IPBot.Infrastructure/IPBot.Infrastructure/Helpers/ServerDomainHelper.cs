namespace IPBot.Infrastructure.Helpers;

public static class ServerDomainHelper
{
    private static readonly string CurrentDomain = DotEnvHelper.EnvironmentVariables["CURRENT_DOMAIN"];

    public static string GetCurrentServerDomain()
    {
        return CurrentDomain.Trim();
    }
}