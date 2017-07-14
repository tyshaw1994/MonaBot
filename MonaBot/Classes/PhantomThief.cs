using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Newtonsoft.Json;

namespace MonaBot.Modules
{
    public class PhantomThief
    {
        public DateTime LastOnlineTime;
        public DateTime NextBedtime;
        public ulong UserId { get; set; }
        public string UserName { get; set; }
        public UserStatus LastStatus { get; set; }

        public PhantomThief(ulong _id, string _name)
        {
            this.UserId = _id;
            this.UserName = _name;
            this.LastOnlineTime = DateTime.UtcNow;
            this.NextBedtime = this.LastOnlineTime.AddHours(7);
            this.LastStatus = UserStatus.Online;
        }
        
        [JsonConstructor]
        public PhantomThief(ulong _id, string _name, DateTime _lastOnline, DateTime _nextBedtime, UserStatus _status)
        {
            this.UserId = _id;
            this.UserName = _name;
            this.LastOnlineTime = _lastOnline;
            this.NextBedtime = _nextBedtime;
            this.LastStatus = _status;
        }

        public override string ToString()
        {
            return String.Format("Name: " + UserName + "\nLast Online Time: " + LastOnlineTime +  "\nNext Bedtime: " + NextBedtime + "\nLast Status: " + LastStatus + "\n\n");
        }
    }
}
