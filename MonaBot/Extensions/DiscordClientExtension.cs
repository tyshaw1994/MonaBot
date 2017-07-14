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

namespace MonaBot.Modules
{
    public static class DiscordClientExtension
    {
        public static List<PhantomThief> UserList(this DiscordSocketClient client, List<PhantomThief> phantomThieves)
        {
            return phantomThieves;
        }
    }
}
