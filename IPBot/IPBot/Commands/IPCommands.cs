﻿namespace IPBot.Commands;

public class IPCommands : ModuleBase
{
    private static readonly string IPFilePath = Path.Combine(Constants.BaseDirectory, @"../latest_ip.txt");

    [Command("ip")]
    public async Task GetIPAsync()
    {
        await Context.Channel.SendMessageAsync(await GetIPFromFileAsync());
    }

    public static async Task<string> GetIPFromFileAsync()
    {
        if (File.Exists(IPFilePath))
        {
            var ip = await File.ReadAllTextAsync(IPFilePath);
            return ip.TrimEnd();
        }

        return string.Empty;
    }
}
