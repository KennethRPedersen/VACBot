using System;
using System.Collections.Generic;
using System.Text;

namespace Discord.Bot.Data.Entities
{
    public class Account
    {
        public string SteamId { get; set; }

        public bool VACBanned { get; set; }
        public DateTime TimeOfReport { get; set; }
        public DateTime TimeOfBan { get; set; }
        public int DaysSinceLastBan { get; set; }
        public int DaysReportToBan { get; set; }


        public ulong ServerId { get; set; }
        public ulong ChannelId { get; set; }
        public ulong MessageId { get; set; }
    }
}
