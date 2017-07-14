using Discord.Commands;
using Discord.WebSocket;
using Discord;
using Microsoft.Extensions.DependencyInjection;
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
using System.Data;

namespace MonaBot.Managers
{
    public class DataManager
    {
        private string userDataFilePath = "../../Data/UserData.json";
        private string channelDataFilePath = "../../Data/ChannelData.json";
        private string channelUserDataFilePath = "../../Data/ChannelUserData.json";
        private MorganaEvents events = new MorganaEvents();

        public async Task ManageUserData(SocketUser user, ISocketMessageChannel channel)
        {
            var userFileData = System.IO.File.ReadAllText(userDataFilePath);
            var channelFileData = System.IO.File.ReadAllText(channelDataFilePath);
            var channelUserFileData = System.IO.File.ReadAllText(channelUserDataFilePath);

            List<PhantomThief> userData = JsonConvert.DeserializeObject<List<PhantomThief>>(userFileData);
            List<PhantomChannel> channelData = JsonConvert.DeserializeObject<List<PhantomChannel>>(channelFileData);
            List<PhantomChannelThief> channelUserData = JsonConvert.DeserializeObject<List<PhantomChannelThief>>(channelUserFileData);

            bool inUsers = false;

            foreach (var u in userData)
            {
                if (u.UserId == user.Id) inUsers = true;
            }

            if (!inUsers)
            {
                userData.Add(new PhantomThief(user.Id, user.Username));
                await events.Log(new LogMessage(LogSeverity.Info, "ManageUserData", "User " + user.Username + " added to UserData store"));
            }

            bool inChannels = false;

            foreach (var ch in channelData)
            {
                if (ch.ChannelId == channel.Id) inChannels = true;
            }

            if (!inChannels)
            {
                channelData.Add(new PhantomChannel
                {
                    ChannelId = channel.Id,
                    ChannelName = channel.Name
                });

                await events.Log(new LogMessage(LogSeverity.Info, "ManageUserData", "Channel " + channel.Name + " added to Channel store"));
            }

            bool inChannelUsers = false;

            foreach (var chu in channelUserData)
            {
                if (chu.ChannelId == channel.Id && chu.UserId == user.Id) inChannelUsers = true;
            }

            if (!inChannelUsers)
            {
                channelUserData.Add(new PhantomChannelThief
                {
                    UserId = user.Id,
                    ChannelId = channel.Id,
                    ChannelUserId = Guid.NewGuid().ToString()
                });

                await events.Log(new LogMessage(LogSeverity.Info, "ManageUserData", "ChannelUser " + channel.Name + ", " + user.Username + " added to ChannelUser store"));
            }

            userFileData = JsonConvert.SerializeObject(userData, Formatting.Indented);
            System.IO.File.WriteAllText(userDataFilePath, userFileData);
            channelFileData = JsonConvert.SerializeObject(channelData, Formatting.Indented);
            System.IO.File.WriteAllText(channelDataFilePath, channelFileData);
            channelUserFileData = JsonConvert.SerializeObject(channelUserData, Formatting.Indented);
            System.IO.File.WriteAllText(channelUserDataFilePath, channelUserFileData);
        }

        public async Task<PhantomThief> GetUserData(SocketMessage message)
        {
            var userFileData = System.IO.File.ReadAllText(userDataFilePath);
            List<PhantomThief> phantomThieves = JsonConvert.DeserializeObject<List<PhantomThief>>(userFileData);

            PhantomThief intendedUser = (from ph in phantomThieves
                                         where ph.UserId == message.Author.Id
                                         select ph).First();

            if (intendedUser == null)
            {
                await ManageUserData(message.Author, message.Channel);
            }

            intendedUser = (from ph in phantomThieves
                            where ph.UserId == message.Author.Id
                            select ph).First();

            if (intendedUser == null)
            {
                throw new Exception("User was not found or created successfully.");
            }

            return intendedUser;
        }

        public async Task<PhantomThief> GetUserData(PhantomThief user, SocketMessage message)
        {
            var userFileData = System.IO.File.ReadAllText(userDataFilePath);
            List<PhantomThief> phantomThieves = JsonConvert.DeserializeObject<List<PhantomThief>>(userFileData);

            PhantomThief intendedUser = (from ph in phantomThieves
                                         where ph.UserId == user.UserId
                                         select ph).First();

            if (intendedUser == null)
            {
                await ManageUserData(message.Author, message.Channel);
            }

            intendedUser = (from ph in phantomThieves
                            where ph.UserId == user.UserId
                            select ph).First();

            if (intendedUser == null)
            {
                throw new Exception("User was not found or created successfully.");
            }

            return intendedUser;
        }

        public async Task<PhantomThief> GetUserData(ulong UserId)
        {
            var userFileData = System.IO.File.ReadAllText(userDataFilePath);
            List<PhantomThief> phantomThieves = JsonConvert.DeserializeObject<List<PhantomThief>>(userFileData);

            PhantomThief intendedUser = (from ph in phantomThieves
                                         where ph.UserId == UserId
                                         select ph).First();

            if (intendedUser == null)
            {
                throw new Exception("User was not found or created successfully.");
            }

            return intendedUser;
        }

        public async Task<List<PhantomThief>> GetAllUserData(IMessageChannel channel)
        {
            var channelUserFileData = System.IO.File.ReadAllText(channelUserDataFilePath);
            List<PhantomChannelThief> channelUserData = JsonConvert.DeserializeObject<List<PhantomChannelThief>>(channelUserFileData);

            List<ulong> userIds = (from chu in channelUserData
                                  where chu.ChannelId == channel.Id
                                  select chu.UserId).ToList();

            List<PhantomThief> intendedUsers = new List<PhantomThief>();

            foreach (var id in userIds)
            {
                intendedUsers.Add(await GetUserData(id));
            }

            return intendedUsers;
        }

        public async Task UpdateUserData(PhantomThief user)
        {
            var userFileData = System.IO.File.ReadAllText(userDataFilePath);
            List<PhantomThief> phantomThieves = JsonConvert.DeserializeObject<List<PhantomThief>>(userFileData);

            PhantomThief intendedUser = (from ph in phantomThieves
                                        where ph.UserId == user.UserId
                                        select ph).First();

            int index = phantomThieves.IndexOf(intendedUser);
            phantomThieves[index] = user;

            userFileData = JsonConvert.SerializeObject(phantomThieves, Formatting.Indented);
            System.IO.File.WriteAllText(userDataFilePath, userFileData);

            await events.Log(new LogMessage(LogSeverity.Info, "UpdateUserData", "User " + user.UserName + " updated in UserData store"));
        }

        public async Task UpdateUserOnlineTime(ulong Id)
        {
            var userFileData = System.IO.File.ReadAllText(userDataFilePath);
            List<PhantomThief> phantomThieves = JsonConvert.DeserializeObject<List<PhantomThief>>(userFileData);

            PhantomThief intendedUser = (from ph in phantomThieves
                                         where ph.UserId == Id
                                         select ph).First();

            intendedUser.LastOnlineTime = DateTime.UtcNow;
            intendedUser.LastStatus = UserStatus.Online;

            int index = phantomThieves.IndexOf(intendedUser);
            phantomThieves[index] = intendedUser;

            userFileData = JsonConvert.SerializeObject(phantomThieves, Formatting.Indented);
            System.IO.File.WriteAllText(userDataFilePath, userFileData);

            //await events.Log(new LogMessage(LogSeverity.Info, "UpdateUserOnlineTime", "User " + intendedUser.UserName + " LastOnlineTime updated in UserData store"));
        }

        public async Task UpdateAllUserStatus(SocketMessage message)
        {
            var channel = message.Channel;
            var usersInChannel = channel.GetUsersAsync().ToList();
            Dictionary<ulong, UserStatus> userStatuses = new Dictionary<ulong, UserStatus>();

            foreach (var u in usersInChannel.Result[0])
            {
                userStatuses.Add(u.Id, u.Status);
            }

            var userFileData = System.IO.File.ReadAllText(userDataFilePath);
            List<PhantomThief> phantomThieves = JsonConvert.DeserializeObject<List<PhantomThief>>(userFileData);

            for(int i = 0; i < phantomThieves.Count; i++)
            {
                if (userStatuses.ContainsKey(phantomThieves[i].UserId))
                {
                    phantomThieves[i].LastStatus = userStatuses[phantomThieves[i].UserId];

                    if(userStatuses[phantomThieves[i].UserId] == UserStatus.Offline)
                    {
                        phantomThieves[i].NextBedtime = phantomThieves[i].LastOnlineTime.AddHours(7);
                    }
                }
            }

            userFileData = JsonConvert.SerializeObject(phantomThieves, Formatting.Indented);
            System.IO.File.WriteAllText(userDataFilePath, userFileData);

            //await events.Log(new LogMessage(LogSeverity.Info, "UpdateAllUserOnlineTimes", "All user statuses up to date in the data store"));
        }
    }
}
