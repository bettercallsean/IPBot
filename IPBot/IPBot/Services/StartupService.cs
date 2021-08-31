using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace IPBot.Services
{
    public class StartupService
    {
        private readonly IServiceProvider _provider;
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly IConfigurationRoot _config;

        public StartupService(IServiceProvider provider, DiscordSocketClient discord, CommandService command, IConfigurationRoot config)
        {
            _provider = provider;
            _discord = discord;
            _commands = command;
            _config = config;
        }

        public async Task StartAsync()
        {
            var token = _config["token"];

            await _discord.LoginAsync(Discord.TokenType.Bot, token);
            await _discord.StartAsync();


            await _commands.AddModulesAsync(Assembly.GetCallingAssembly(), _provider);
        }
    }
}
