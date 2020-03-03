using Discord.Bot.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Discord.Bot.Data.Repos.Interfaces
{
    public interface ISteamAPIRepo
    {
        /// <summary>
        /// Checks a list of accounts, and update their VAC status, given any updates exists.
        /// </summary>
        /// <param name="accounts"></param>
        /// <returns>A list of accounts with their updated VAC status.</returns>
        List<Account> checkAccounts(List<Account> accounts);

        /// <summary>
        /// Checks if an account exists.
        /// </summary>
        /// <param name="account"></param>
        /// <returns>An <see cref="Account"/> with it's current VAC status</returns>
        bool validateAccount(Account account);

    }
}
