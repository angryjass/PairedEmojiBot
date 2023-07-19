using Microsoft.EntityFrameworkCore;
using PairedEmojiBot.Abstract;
using PairedEmojiBot.Db;
using PairedEmojiBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace PairedEmojiBot.Concrete.InitHandlers
{
    public class CrayfishEmojiInitHandler : BaseEmojiHandler
    {
        public override string GetEmojiCommand()
        {
            return "/crayfish";
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

                if (message.Text == null)
                    throw new Exception("Message.Text is null!");

                if (message.From == null)
                    throw new Exception("Message.From is null!");

                var arr = message.Text.Replace("@paired_emoji_bot", "").Split('@');
                var firstUser = message.From.Username;
                var secondUser = arr.Length > 1 ? arr[1].Trim() : null;

                InlineKeyboardMarkup getApprove = new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("Готов", "crayfishGame"));

                Message? outMessage;

                if (secondUser == null)
                    outMessage = await botClient.SendTextMessageAsync(message.Chat, $"Господин @{firstUser} пожелал прожарить кого-нибудь из этой группы! Кто же осмелиться выйти с ним на поединок? Нажмите Готов, чтобы начать дуэль!", replyMarkup: getApprove);
                else
                    outMessage = await botClient.SendTextMessageAsync(message.Chat, $"Господин @{firstUser} пожелал прожарить господина @{secondUser}! Нажмите Готов, чтобы начать дуэль!", replyMarkup: getApprove);

                await botClient.SendTextMessageAsync(message.Chat, "Ожидание готовности игроков...");

                await context.AddAsync(new CrayfishGameProcess()
                {
                    FirstUsername = firstUser,
                    SecondUsername = secondUser,
                    ChatId = outMessage.Chat.Id,
                    MessageId = outMessage.MessageId,
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
