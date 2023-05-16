using PairedEmojiBot.Abstract;
using PairedEmojiBot.Db;
using PairedEmojiBot.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace PairedEmojiBot.Concrete.InitHandlers
{
    public class HelpHandler : BaseEmojiHandler
    {
        public override string GetEmojiCommand()
        {
            return "/help";
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

                await botClient.SendTextMessageAsync(message.Chat, "Инструкция по использованию бота:\n" +
                    $"Отправь /handshake если хочешь, чтобы тебя поддержал твой братишка после прохладной шутки {EmojiEnum.HANDSHAKE_EMOJI}\n" +
                    $"Отправь /five если хочешь собрать пятюни со своих братишек {EmojiEnum.FIVE_EMOJI}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
