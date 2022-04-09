namespace IPBot.Commands;

public class IPCommands : InteractionModuleBase<SocketInteractionContext>
{
    private static readonly string IPFilePath = Path.Combine(Constants.BaseDirectory, @"../latest_ip.txt");

    [SlashCommand("ip", "get the current IP of the server")]
    public async Task GetIPAsync()
    {
        var ip = await GetIPFromFileAsync();
        await RespondAsync(ip);
    }

    public static async Task<string> GetIPFromFileAsync()
    {
        if (!File.Exists(IPFilePath)) return "Couldn't find IP";
        
        var ip = await File.ReadAllTextAsync(IPFilePath);
        return ip.TrimEnd();
    }
}