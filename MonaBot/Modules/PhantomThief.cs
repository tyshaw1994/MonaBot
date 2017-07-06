using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace MonaBot.Modules
{
    public class PhantomThief
    {
        public DateTime logonTime;
        public DateTime lastTold;
        private ulong id;
        public string name;

        public PhantomThief(ulong _id, string _name)
        {
            this.id = _id;
            this.name = _name;
            this.logonTime = DateTime.UtcNow;
            this.lastTold = DateTime.UtcNow;
        }

        public ulong GetId()
        {
            return this.id;
        }
    }
}
