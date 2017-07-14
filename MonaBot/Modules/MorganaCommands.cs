using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using MonaBot;
using MonaBot.Modules;
using MonaBot.Managers;

namespace MonaBot.Modules
{
    /// <summary>
    /// Bot requirements:
    /// -Randomly say "SASUGA @whoever" when someone makes the 100th message every 100msgs
    /// -"WOAH, LOOKING COOL @whoever" every 1000 msgs
    /// -Randomly tells people to go to sleep
    /// </summary>

    public class MorganaCommands : ModuleBase
    {
        DataManager manager = new DataManager();

        [Command("pet"), Summary("Pets Mona.")]
        public async Task Pet()
        {
            await ReplyAsync("nyaa~");
        }

        [Command("help"), Summary("Gets list of commands.")]
        public async Task Help()
        {
            await ReplyAsync("I currently only have one command: !pet");
        }

        [Command("user"), Summary("Prints list of users in server")]
        public async Task UserList()
        {
            List<PhantomThief> phantomThieves = await manager.GetAllUserData(Context.Channel);

            string users = "";

            foreach (var ph in phantomThieves)
            {
                users = String.Concat(users, ph.ToString());
            }

            await ReplyAsync("List of users who have posted in the current channel: \n" + "```" + users + "```");
        }
    }

}
