using ConsoleTables;
using Discord.Bot.Data.Entities;
using Discord.Bot.Data.Repos.Interfaces;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Discord.Bot.Modules
{
    /// <summary>
    /// This module is used to set/remove the rank of a user.
    /// </summary>
    [Name("VAC Track")]
    [Summary("Commands regarding tracking users bound to be VAC'd")]
    public class Module : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _service;
        private readonly IConfigurationRoot _config;
        private readonly ISteamAPIRepo _steamRepo;
        private readonly IAccountRepo _accRepo;

        public Module(CommandService service, IConfigurationRoot config, ISteamAPIRepo steamAPIRepo, IAccountRepo accRepo)
        {
            _service = service;
            _config = config;
            _steamRepo = steamAPIRepo;
            _accRepo = accRepo;        }

        [Command("VAC")]
        [Summary("Starts tracking specified account for VAC bans.")]
        public async Task AddAccount(Int64 steamid)
        {
            Regex r = new Regex("^[0-9]{17}$");
            
            if(!r.Match(steamid.ToString()).Success)
            {
                await ReplyAsync("Invalid SteamId64. Please enter a valid one (e.g. 76561198029915406)");
                return;
            }

            Account acc = new Account()
            {
                SteamId = steamid.ToString(),
                TimeOfReport = DateTime.Now,
                VACBanned = false,
                ServerId = Context.Guild.Id,
                ChannelId = Context.Channel.Id,
                MessageId = Context.Message.Id
            };

            try
            {
                bool exist = _steamRepo.validateAccount(acc);
                if(exist)
                {
                    _accRepo.AddAccount(acc);

                    string message = "User is now being tracked. \nUpdating tracked users...";
                    await ReplyAsync(message);

                    await UpdateVACStatus();
                }
            }
            catch (Exception e)
            {
                await ReplyAsync($"Error: {e.Message}");
                return;
            }
        }

        [Command("nonbanned")]
        [Summary("Shows the longest nonbanned accounts paged.")]
        public async Task ListNonbannedAccounts(int page = 0)
        {
            var accs = _accRepo.GetLatestNonbannedAccounts(GetServerId(), 10, page);

            printAccounts(accs, "Non-banned accounts: ");
        }

        [Command("banned")]
        [Summary("Shows the latest non-banned accounts paged.")]
        public async Task ListBannedAccounts(int page = 0)
        {
            var accs = _accRepo.GetLatestBannedAccounts(GetServerId(), 10, page);

            printAccounts(accs, "Banned accounts: ");
        }

        [Command("update")]
        [Summary("Updates the VAC status of all non-banned accounts.")]
        public async Task UpdateVACStatus()
        {
            var totalCount = 0;
            List<Account> accs = new List<Account>();
            try
            {
                accs = _accRepo.GetNonbannedAccounts(GetServerId());
                if(accs.Count() > 0)
                {
                    totalCount = accs.Count();
                    accs = _steamRepo.checkAccounts(accs).Where(acc => acc.VACBanned == true).ToList();
                }

            }
            catch (Exception e)
            {
                await ReplyAsync($"Error: {e.Message}");
                return;
            }

            // If no tracked accounts are currently not banned
            if (totalCount == 0)
            {
                await ReplyAsync("All currently tracked accounts are banned! :-)");
                return;
            }

            _accRepo.UpdateAccounts(accs);
            await ReplyAsync($"Out of {totalCount} tracked, non-banned account(s), {accs.Count()} account(s) has now been banned!");
            printAccounts(accs, "Newly banned accounts: ");

        }

        /// <summary>
        /// Prints the given list of accounts as a table.
        /// </summary>
        /// <param name="accs">The list of <see cref="Account"/></param>
        /// <param name="message">The message to display before the table</param>
        private async void printAccounts(List<Account> accs, string message)
        {
            if (accs.Count() > 0)
            {
                var table = new ConsoleTable("SteamID", "VAC Status", "Reported on", "Banned on", "Time Report -> Ban");

                accs.ForEach(acc =>
                {
                    table.AddRow(acc.SteamId, acc.VACBanned, acc.TimeOfReport, acc.TimeOfBan, acc.DaysReportToBan + " Days");
                });

                await ReplyAsync($"{message}\n```{table.ToMarkDownString()}```");
            } else
            {
                await ReplyAsync("No tracked accounts found!");
            }
        }

        private ulong GetServerId()
        {
            return Context.Guild.Id;
        }
    }

}
