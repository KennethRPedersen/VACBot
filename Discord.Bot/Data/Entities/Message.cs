using System;
using System.Collections.Generic;
using System.Text;

namespace Discord.Bot.Data.Entities
{
    public class Message
    {
        public ulong ServerId { get; set; }
        public ulong ChannelId { get; set; }
        public ulong MessageId { get; set; }

        public Account Account { get; set; }
        public string SteamId { get; set; }
    }
}
