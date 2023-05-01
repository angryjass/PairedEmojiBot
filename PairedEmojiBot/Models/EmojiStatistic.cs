namespace PairedEmojiBot.Models
{
    public class EmojiStatistic
    {
        public int Id { get; set; }
        public long ChatId { get; set; }
        public long MessageId { get; set; }
        public DateTime Created { get; set; }
        public long Count { get; set; }

        public virtual ICollection<User> Users { get; set; } = new HashSet<User>();
    }
}
