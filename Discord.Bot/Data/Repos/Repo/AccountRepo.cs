using Discord.Bot.Data.Entities;
using Discord.Bot.Data.Repos.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord.Bot.Data.Repos.Repo
{
    public class AccountRepo : IAccountRepo
    {

        private readonly DataContext _ctx;

        public AccountRepo(DataContext ctx)
        {
            _ctx = ctx;
        }

        public bool CheckAccountExist(string id)
        {
            return _ctx.Accounts.Where(acc => acc.SteamId == id).Count() > 0;
        }

        public void AddAccount(Account account)
        {
            if (CheckAccountExist(account.SteamId)) throw new Exception("Account is already being tracked!");
            _ctx.Accounts.Attach(account).State = EntityState.Added;
            _ctx.SaveChanges();
        }

        public List<Account> GetNonbannedAccounts()
        {
            return _ctx.Accounts.Where(acc => acc.VACBanned == false)
                .Include(acc => acc.Message)
                .ToList();
        }

        public void UpdateAccounts(List<Account> accs)
        {
            accs.ForEach(acc =>
            {
                _ctx.Accounts.Attach(acc).State = EntityState.Modified;
            });
            _ctx.SaveChanges();
        }

        public List<Account> GetLatestNonbannedAccounts(int amount, int prev = 0)
        {
            return _ctx.Accounts.Where(acc => acc.VACBanned == false)
                .OrderBy(acc => acc.TimeOfReport)
                .Skip((prev - 1) * amount)
                .Take(amount).ToList();
        }

        public List<Account> GetLatestBannedAccounts(int amount, int prev = 0)
        {
            return _ctx.Accounts.Where(acc => acc.VACBanned == true)
                .OrderByDescending(acc => acc.TimeOfReport)
                .Skip((prev - 1) * amount)
                .Take(amount).ToList();
        }
    }
}
