using PairedEmojiBot.Abstract;

namespace PairedEmojiBot.Classifiers
{
    public class ReplyDataClassifier : BaseClassifier
    {
        public ReplyDataClassifier(Dictionary<string, Delegate> handlers, Telegram.Bot.Types.Update update) : base(handlers, update) { }
        public override Delegate? Classify()
        {
            foreach (var handler in _handlers)
            {
                if (_update.CallbackQuery == null)
                    continue;

                if (_update.CallbackQuery.Data == null)
                    continue;

                if (_update.CallbackQuery.Data.Contains(handler.Key))
                    return handler.Value;
            }

            return null;
        }
    }
}
