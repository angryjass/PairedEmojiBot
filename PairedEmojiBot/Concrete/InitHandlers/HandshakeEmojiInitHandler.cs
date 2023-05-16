using PairedEmojiBot.Abstract;
using PairedEmojiBot.Db;
using PairedEmojiBot.Enums;
using PairedEmojiBot.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace PairedEmojiBot.Concrete.InitHandlers
{
    public class HandshakeEmojiInitHandler : BaseEmojiHandler
    {
        public override string GetEmojiCommand()
        {
            return "/handshake";
        }

        public override UpdateType GetRequestType()
        {
            return UpdateType.Message;
        }

        public override async Task Handle(PairedEmojiBotContext context, ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                var message = update.Message ?? throw new Exception("Message is null");

                if (message.From == null)
                    throw new Exception("Message.From is null!");

                InlineKeyboardMarkup getHandShake = new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("Дать краба", StringUtils.GetUserFIOAndUsername(message.From) + ";" + message.From.Username));
                await botClient.SendTextMessageAsync(message.Chat, EmojiEnum.REQUEST_HANDSHAKE_EMOJI, replyMarkup: getHandShake);
                await botClient.SendTextMessageAsync(message.Chat, "И имя красавчика.....");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            };
        }
    }
}
