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
    public class MorganaEvents
    {
        private CommandService commands;
        private MorganaFeatures feature = new MorganaFeatures();

        public Dictionary<ulong, int> channelMessageCounts = new Dictionary<ulong, int>();

        public async Task MessageReceived(DiscordSocketClient client, SocketMessage message, DataManager manager)
        {
            if (message.Source == MessageSource.Bot) return;
            else
            {
                await manager.ManageUserData(message.Author, message.Channel); // Add User, Channel, ChannelUser to data store if not in there yet
                await manager.UpdateUserOnlineTime(message.Author.Id); // Update the current user's online time
                await manager.UpdateAllUserStatus(message); // Update all current user statuses in data store

                bool inList = (from cmc in channelMessageCounts
                               where cmc.Key == message.Channel.Id
                               select cmc).Any();

                if (!inList)
                {
                    channelMessageCounts.Add(message.Channel.Id, 0);
                }

                channelMessageCounts[message.Channel.Id]++;
                int currentMessageCount = channelMessageCounts[message.Channel.Id];

                await feature.SasugaJokah(message, currentMessageCount);
                await feature.GoToBed(message, manager);

                if (feature.monaMessages > 100)
                {
                    // maybe implement some kind of wiping/rate limiting
                }
            }
        }

        public async Task UserJoined(SocketGuildUser user)
        {
            Console.WriteLine("User " + user.Username + " joined at " + DateTime.UtcNow.ToLocalTime());

            //await ManageUserList(user, 1);      
        }

        public async Task UserLeft(SocketGuildUser user)
        {
            Console.WriteLine("User " + user.Username + " left at " + DateTime.UtcNow.ToLocalTime());
            //await ManageUserList(user, 0);
        }

        public async Task UserUpdated(SocketUser userOld, SocketUser userNew)
        {
            Console.WriteLine("Old Status: " + userOld.Status + "\nNew Status: " + userNew.Status);

            if (userNew.Status == UserStatus.Online)
                Console.WriteLine("Hi " + userNew.Username);
            else if (userNew.Status == UserStatus.DoNotDisturb)
                Console.WriteLine("I won't disturb you.");
        }

        public async Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            //return Task.FromResult(false);
        }

        #region Legacy Code
        // Deprecated: No longer using List to keep track of members so that data isn't lost upon restart
        //public async Task ManageUserList(SocketGuildUser user, int status)
        //{
        //    PhantomThief currentUser = (from pt in phantomThieves
        //                                where pt.UserId == user.Id
        //                                select pt).First();
        //    if (currentUser == null)
        //        return;

        //    switch (status)
        //    {
        //        case 0: // user has left
        //            phantomThieves.Remove(currentUser);
        //            break;
        //        case 1: // user has joined
        //            phantomThieves.Add(new PhantomThief(user.Id, user.Username));
        //            break;
        //        default:
        //            Console.WriteLine("Invalid call to ManageUserList at " + DateTime.UtcNow);
        //            break;
        //    }
        //}
        #endregion Legacy Code
    }
}
