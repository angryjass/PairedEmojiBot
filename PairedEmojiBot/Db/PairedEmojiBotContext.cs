using Microsoft.EntityFrameworkCore;
using PairedEmojiBot.Models;

namespace PairedEmojiBot.Db
{
    public class PairedEmojiBotContext : DbContext
    {
        public DbSet<EmojiStatistic> EmojiStatistics { get; set; }
        public DbSet<User> Users { get; set; }
        public PairedEmojiBotContext()
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=DbFiles/paired_emoji_bot.db;Mode=ReadWriteCreate");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EmojiStatistic>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<EmojiStatistic>()
                .HasMany(x => x.Users)
                .WithOne(x => x.EmojiStatistic)
                .HasForeignKey(x => x.EmojiStatisticId);



            modelBuilder.Entity<User>()
                .HasKey(x => x.Id);
        }
    }
}
