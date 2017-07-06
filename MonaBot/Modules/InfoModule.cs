using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;

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
        [Command("pet"), Summary("Pets Mona.")]
        public async Task Pet()
        {
            await ReplyAsync("nyaa~");
        }
    }

}
