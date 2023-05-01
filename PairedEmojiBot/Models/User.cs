namespace PairedEmojiBot.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Username { get; set; }
        public int EmojiStatisticId { get; set; }
        public int ReactionsCount { get; set; }

        public virtual EmojiStatistic EmojiStatistic { get; set; } = new EmojiStatistic();
    }
}
