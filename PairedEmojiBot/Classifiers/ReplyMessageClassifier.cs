using PairedEmojiBot.Abstract;

namespace PairedEmojiBot.Classifiers
{
    public class ReplyMessageClassifier : BaseClassifier
    {
        public ReplyMessageClassifier(Dictionary<string, Delegate> handlers, Telegram.Bot.Types.Update update) : base(handlers, update) { }

        public override Delegate? Classify()
        {
            foreach (var handler in _handlers)
            {
                if (_update.CallbackQuery == null)
                    continue;

                if (_update.CallbackQuery.Message == null)
                    continue;

                if (_update.CallbackQuery.Message.Text == null)
                    continue;

                if (_update.CallbackQuery.Message.Text.Contains(handler.Key))
                    return handler.Value;
            }

            return null;
        }
    }
}
