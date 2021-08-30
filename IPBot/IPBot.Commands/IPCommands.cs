using System;
using System.IO;
using System.Threading.Tasks;
using Discord.Commands;
using IPBot.Infrastructure;

namespace IPBot.Commands
{
    public class IPCommands : ModuleBase
    {
        [Command("ip")]
        public async Task GetIP()
        {
            var ipFilePath = Path.Combine(Constants.BaseDirectory, @"../latest_ip.txt");
            Console.WriteLine(ipFilePath);

            if (File.Exists(ipFilePath))
            {
                var ip = await File.ReadAllTextAsync(ipFilePath);
                await Context.Channel.SendMessageAsync(ip);
            }
        }
    }
}
