namespace IPBot.Commands;

public class IPCommands : ModuleBase
{
    private static readonly string IPFilePath = Path.Combine(Constants.BaseDirectory, @"../latest_ip.txt");

    [Command("ip")]
    public async Task GetIP()
    {
        await Context.Channel.SendMessageAsync(await GetIPFromFile());
    }

    public static async Task<string> GetIPFromFile()
    {
        if (File.Exists(IPFilePath))
        {
            return await File.ReadAllTextAsync(IPFilePath);
        }

        return string.Empty;
    }
}
