using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord;
using Discord.WebSocket;
using System.Configuration;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MonaBot.Modules;
using MonaBot.Managers;

namespace MonaBot
{
    public class Program
    {
        private DiscordSocketClient client;
        private CommandService commands;
        private IServiceProvider services;
        private MorganaEvents events = new MorganaEvents();

        public DataManager manager;

        public static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            manager = new DataManager();
            commands = new CommandService();
            client = new DiscordSocketClient();

            client.Log += events.Log;
            client.MessageReceived += MessageReceived;
            client.UserJoined += events.UserJoined;
            client.UserLeft += events.UserLeft;
            client.UserUpdated += events.UserUpdated;

            await InstallCommands();

            string token = "MzMyMjM0ODAwMTI5NjM4NDAx.DD7K6g.knN_OVDGIj7og8d_XDsKuhSmhi4";
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();
            Console.WriteLine("Mona started at " + DateTime.UtcNow);

            await Task.Delay(-1);
        }

        public async Task InstallCommands()
        {
            MorganaCommands monaCmd = new MorganaCommands();
            Type type = monaCmd.GetType();
            await commands.AddModuleAsync(type);
        }

        public async Task HandleCommand(DiscordSocketClient client, SocketMessage messageParam)
        {
            if (messageParam.Source == MessageSource.Bot) return;

            var message = messageParam as SocketUserMessage;
            if (message == null) return;

            int argPos = 0;

            if (!message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(client.CurrentUser, ref argPos)) return;
            var context = new CommandContext(client, message);
            var result = await commands.ExecuteAsync(context, argPos, services);
            if (!result.IsSuccess)
                await context.Channel.SendMessageAsync(result.ErrorReason);
        }

        public async Task MessageReceived(SocketMessage message)
        {
            
            if (message.Source == MessageSource.Bot) return;
            else
            {
                if(message.ToString().Substring(0, 1) == "!")
                    await HandleCommand(client, message);
                else
                    await events.MessageReceived(client, message, manager);
            }
        }
    }
}
