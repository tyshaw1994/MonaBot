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

namespace MonaBot
{
    public class Program
    {
        private CommandService commands;
        private DiscordSocketClient client;
        private IServiceProvider services;
        private List<PhantomThief> phantomThieves;

        public int msgCount = 0;

        public static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            client = new DiscordSocketClient();
            client.Log += Log;
            client.MessageReceived += MessageReceived;
            //client.UserJoined += UserJoined;
            

            phantomThieves = new List<PhantomThief>();

            commands = new CommandService();
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

        public async Task HandleCommand(SocketMessage messageParam)
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

        private async Task MessageReceived(SocketMessage message)
        {
            bool inList = (from ph in phantomThieves
                           where ph.GetId() == message.Author.Id
                           select ph).Any();

            if (!inList)
            {
                phantomThieves.Add(new PhantomThief(message.Author.Id, message.Author.Username));
            }

            //Console.WriteLine(phantomThieves.ElementAt(0).GetLogonTime());

            msgCount++;
            //Console.WriteLine(msgCount);
            await SasugaJokah(message);
            await GoToBed(message);

            if (message.Source != MessageSource.Bot) await HandleCommand(message);
        }

        private async Task UserJoined(SocketGuildUser user)
        {
            phantomThieves.Add(new PhantomThief(user.Id, user.Username));
        }

        private async Task SasugaJokah(SocketMessage message)
        {
            if (msgCount % 100 == 0 && msgCount % 1000 != 0)
            {
                await message.Channel.SendFileAsync("MonaSasuga.png", "SASUGA " + message.Author.Mention.ToUpper());
                Console.WriteLine("User " + message.Author.Username + " got the 100th post.");
            }
            else if (msgCount % 1000 == 0)
            {
                await message.Channel.SendFileAsync("MonaCool.png", "WOAH, LOOKING COOL " + message.Author.Mention.ToUpper());
                Console.WriteLine("User " + message.Author.Username + " got the 1000th post.");
            }
        }

        private async Task GoToBed(SocketMessage message)
        {
            PhantomThief user = (from ph in phantomThieves
                       where ph.GetId() == message.Author.Id
                       select ph).First();

            DateTime logon = user.logonTime;
            double timeOnline = (DateTime.UtcNow - logon).TotalHours;

            // If the user has been online for more than 6 hours, tell them to go to bed
            if (timeOnline > 6)
            {
                user.lastTold = DateTime.UtcNow;
                await message.Channel.SendFileAsync("GoToBed.jpg", message.Author.Mention);
                Console.WriteLine("User " + user.name + " was told to go to bed at " + DateTime.UtcNow);
            }

            // If the user hasn't been told go to bed in 12 hours, tell them to go to bed
            double timeSinceTold = (DateTime.UtcNow - user.lastTold).TotalHours;
            if (timeSinceTold > 12)
            {
                user.lastTold = DateTime.UtcNow;
                await message.Channel.SendFileAsync("GoToBed.jpg", message.Author.Mention);
                Console.WriteLine("User " + user.name + " was told to go to bed at " + DateTime.UtcNow);
            }
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.FromResult(false);
        }
    }
}
