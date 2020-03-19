using Discord.Bot.Data.Entities;
using Discord.Bot.Data.Repos.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Bot.Data.Repos.Repo
{
    class SteamAPIRepo : ISteamAPIRepo
    {
        public bool validateAccount(Account account)
        {
            string url = $"http://api.steampowered.com/ISteamUser/GetPlayerBans/v1/?key=C15D224F522B82D6EB0ADB72BA015EDA&steamids={account.SteamId}";
            List<Account> accs = new List<Account>();
            using (var client = new WebClient())
            using (var stream = client.OpenRead(url))
            using (var reader = new StreamReader(stream))
            {
                var jObject = Newtonsoft.Json.Linq.JObject.Parse(reader.ReadLine());
                accs = JsonConvert.DeserializeObject<List<Account>>(jObject["players"].ToString());
            }

            if(accs.Count < 1)
            {
                throw new Exception("No user exists with given ID!");
            }

            return true;
        }

        public List<Account> checkAccounts(List<Account> accounts)
        {
            accounts = checkForBans(accounts);

            return accounts;
        }

        private List<Account> checkForBans(List<Account> accounts) {

            string ids = "";
            List<Account> accs = new List<Account>();

            accounts.ForEach(acc =>
            {
                ids += $"{acc.SteamId},";
            });
            ids.Remove(ids.Length - 1);

            string url = $"http://api.steampowered.com/ISteamUser/GetPlayerBans/v1/?key=C15D224F522B82D6EB0ADB72BA015EDA&steamids={ids}";

            using (var client = new WebClient())
            using (var stream = client.OpenRead(url))
            using (var reader = new StreamReader(stream))
            {
                var jObject = Newtonsoft.Json.Linq.JObject.Parse(reader.ReadLine());
                accs = JsonConvert.DeserializeObject<List<Account>>(jObject["players"].ToString());
            }


            accounts.ForEach(acc =>
            {
                Account apiAcc = accs.FirstOrDefault(a => a.SteamId == acc.SteamId);

                if (apiAcc != null)
                {
                    if ((acc.VACBanned == false && apiAcc.VACBanned == true) || (acc.DaysSinceLastBan != apiAcc.DaysSinceLastBan))
                    {
                        acc.VACBanned = true;
                        acc.TimeOfBan = DateTime.Now.AddDays(-apiAcc.DaysSinceLastBan);
                        acc.DaysReportToBan = new TimeSpan(acc.TimeOfBan.Ticks - acc.TimeOfReport.Ticks).Days;
                    }
                }
            });

            return accounts;
        }
    }
}
