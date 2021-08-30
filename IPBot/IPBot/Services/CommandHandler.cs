using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using IPBot.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace IPBot.Services
{
    public class CommandHandler
    {
        public static IServiceProvider _provider;
        public static DiscordSocketClient _discord;
        public static CommandService _commands;
        public static IConfigurationRoot _config;

        public CommandHandler(IServiceProvider provider, DiscordSocketClient discord, CommandService command, IConfigurationRoot config)
        {
            _provider = provider;
            _discord = discord;
            _commands = command;
            _config = config;

            _discord.Ready += OnReady;
            _discord.MessageReceived += OnMessageReceived;
        }

        private async Task OnMessageReceived(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;

            if (message.Author.IsBot)
                return;

            var context = new SocketCommandContext(_discord, message);

            int pos = 0;
            if (message.HasStringPrefix(_config["prefix"], ref pos) || message.HasMentionPrefix(_discord.CurrentUser, ref pos))
            {
                var result = await _commands.ExecuteAsync(context, pos, _provider);

                if (!result.IsSuccess)
                    Console.WriteLine($"{result.Error}");
            }
        }

        private Task OnReady()
        {
            Console.WriteLine($"Logged in as {_discord.CurrentUser.Username}#{_discord.CurrentUser.Discriminator}");


            Console.WriteLine(Constants.BaseDirectory);
            return Task.CompletedTask;
        }
    }
}
