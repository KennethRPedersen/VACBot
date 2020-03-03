using Discord.Bot.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Discord.Bot.Data.Repos.Interfaces
{
    public interface IAccountRepo
    {
        void AddAccount(Account account);
        List<Account> GetNonbannedAccounts();
        void UpdateAccounts(List<Account> accs);
        List<Account> GetLatestNonbannedAccounts(int amount, int prev);
        List<Account> GetLatestBannedAccounts(int amount, int prev);
    }
}
