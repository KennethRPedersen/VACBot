using Discord.Bot.Data.Entities;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Discord.Bot.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> dbContextOptions) : base(dbContextOptions)
        {
            // USE BELOW TO RESET DB
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Message>()
                .HasKey(ent => ent.MessageId);

            modelBuilder.Entity<Account>()
                .HasKey(ent => ent.SteamId);

            modelBuilder.Entity<Account>()
                .HasOne(acc => acc.Message)
                .WithOne(msg => msg.Account)
                .HasForeignKey<Message>(msg => msg.SteamId);
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Message> Messages { get; set; }
    }
}
