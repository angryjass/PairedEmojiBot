using Microsoft.EntityFrameworkCore;
using PairedEmojiBot.Abstract;
using PairedEmojiBot.Db;
using PairedEmojiBot.Enums;
using PairedEmojiBot.Models;
using PairedEmojiBot.Utils;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace PairedEmojiBot.Concrete.ProcessHandlers
{
    public class FiveEmojiProcessHandler : BaseEmojiHandler
    {
        private static InlineKeyboardMarkup getFive = new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("Дать пять", "Дал пять"));
        public override string GetEmojiCommand()
        {
            return EmojiEnum.FIVE_EMOJI;
        }

        public override UpdateType GetRequestType()
        {
            return UpdateType.CallbackQuery;
        }

        public override async Task Handle(PairedEmojiBotContext context, ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                var callbackQuery = update.CallbackQuery ?? throw new Exception("CallbackQuery is null");
                var message = callbackQuery.Message ?? throw new Exception("CallbackQuery.Message is null");
                var data = callbackQuery.Data ?? throw new Exception("CallbackQuery.Data is null");
                var emojiStatistic = await context.Set<EmojiStatistic>().Include(a => a.Users).FirstAsync(a => a.ChatId == message.Chat.Id && a.MessageId == message.MessageId + 1);
                emojiStatistic.Count++;
                var messageBuilder = new StringBuilder();
                messageBuilder.AppendLine("Количество пятюнь: " + emojiStatistic.Count);

                if (update.CallbackQuery.From.Username != null ? !emojiStatistic.Users.Any(a => a.Username == update.CallbackQuery.From.Username) : !emojiStatistic.Users.Any(a => a.FirstName + a.LastName == update.CallbackQuery.From.FirstName + update.CallbackQuery.From.LastName))
                    emojiStatistic.Users.Add(
                        new Models.User()
                        {
                            FirstName = update.CallbackQuery.From.FirstName,
                            LastName = update.CallbackQuery.From.LastName,
                            Username = update.CallbackQuery.From.Username,
                            ReactionsCount = 0
                        });

                foreach (var user in emojiStatistic.Users)
                {
                    if (update.CallbackQuery.From.Username != null ? update.CallbackQuery.From.Username == user.Username : update.CallbackQuery.From.FirstName + update.CallbackQuery.From.LastName == user.FirstName + user.LastName)
                        user.ReactionsCount++;

                    messageBuilder.AppendLine(StringUtils.GetUserFIOAndUsername(user) + " - " + user.ReactionsCount);
                }

                await botClient.EditMessageTextAsync(new ChatId(message.Chat.Id), message.MessageId + 1, messageBuilder.ToString());

                await botClient.EditMessageTextAsync(new ChatId(message.Chat.Id), message.MessageId, EmojiEnum.OPPOSITE_FIVE_EMOJI, replyMarkup: getFive);
                Thread.Sleep(700);
                await botClient.EditMessageTextAsync(new ChatId(message.Chat.Id), message.MessageId, EmojiEnum.FIVE_EMOJI, replyMarkup: getFive);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
