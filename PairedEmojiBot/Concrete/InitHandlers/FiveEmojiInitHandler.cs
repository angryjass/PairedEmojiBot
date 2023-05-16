using Microsoft.EntityFrameworkCore;
using PairedEmojiBot.Abstract;
using PairedEmojiBot.Db;
using PairedEmojiBot.Enums;
using PairedEmojiBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace PairedEmojiBot.Concrete.InitHandlers
{
    public class FiveEmojiInitHandler : BaseEmojiHandler
    {
        private InlineKeyboardMarkup getFive = new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("Дать пять", "Дал пять"));
        public override string GetEmojiCommand()
        {
            return "/five";
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
                await botClient.SendTextMessageAsync(message.Chat, EmojiEnum.FIVE_EMOJI, replyMarkup: getFive);

                var outputMessage = await botClient.SendTextMessageAsync(message.Chat, "Количество пятюнь: 0");

                await context.AddAsync(
                    new EmojiStatistic()
                    {
                        ChatId = outputMessage.Chat.Id,
                        MessageId = outputMessage.MessageId,
                        Count = 0,
                        Created = DateTime.UtcNow
                    });

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
