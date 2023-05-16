using PairedEmojiBot.Db;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace PairedEmojiBot.Abstract
{
    public abstract class BaseEmojiHandler
    {
        public abstract Task Handle(PairedEmojiBotContext context, ITelegramBotClient botClient, Update update, CancellationToken cancellationToken);
        public abstract string GetEmojiCommand();
        public abstract Telegram.Bot.Types.Enums.UpdateType GetRequestType();
    }
}
