namespace IPBot.Infrastructure.Helpers;

public static class ServerDomainHelper
{
    private static readonly string IPFilePath = Path.Combine(AppContext.BaseDirectory, @"../current_domain.txt");

    public static async Task<string> GetCurrentServerDomainAsync()
    {
        var serverDomain = await File.ReadAllTextAsync(IPFilePath);
        return serverDomain.Trim();
    }
}