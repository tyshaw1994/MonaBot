using Discord.Commands;
using Discord.WebSocket;
using Discord;
using Microsoft.Extensions.DependencyInjection;
using MonaBot.Managers;
using MonaBot.Modules;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System;

namespace MonaBot.Modules
{
    public class MorganaFeatures
    {
        public int monaMessages = 0;

        public async Task SasugaJokah(SocketMessage message, int msgCount)
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

            monaMessages++;
        }

        public async Task GoToBed(SocketMessage message, DataManager manager)
        {
            PhantomThief user = await manager.GetUserData(message);

            // If the user has been online for more than 7 hours, tell them to go to bed
            if (user.NextBedtime < DateTime.UtcNow)
            {
                user.LastOnlineTime = DateTime.UtcNow;
                user.NextBedtime = user.LastOnlineTime.AddHours(7);
                await manager.UpdateUserData(user);
                await message.Channel.SendFileAsync("GoToBed.jpg", message.Author.Mention);
                Console.WriteLine("User " + user.UserName + " was told to go to bed at " + DateTime.UtcNow);
            }

            monaMessages++;
        }
    }
}
