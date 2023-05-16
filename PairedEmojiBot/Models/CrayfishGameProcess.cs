namespace PairedEmojiBot.Models
{
    public class CrayfishGameProcess
    {
        public int Id { get; set; }
        public long ChatId { get; set; }
        public long MessageId { get; set; }
        public string FirstUsername { get; set; }
        public string? SecondUsername { get; set; }
        public bool ApproveFirstUser { get; set; }
        public bool ApproveSecondUser { get; set; }
        public string? Winner { get; set; }
    }
}
