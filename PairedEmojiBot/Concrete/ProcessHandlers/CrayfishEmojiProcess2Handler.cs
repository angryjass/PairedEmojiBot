using Microsoft.EntityFrameworkCore;
using PairedEmojiBot.Abstract;
using PairedEmojiBot.Db;
using PairedEmojiBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace PairedEmojiBot.Concrete.ProcessHandlers
{
    public class CrayfishEmojiProcess2Handler : BaseEmojiHandler
    {
        public override string GetEmojiCommand()
        {
            return "BOOM!";
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
                var from = callbackQuery.From.Username ?? throw new Exception("CallbackQuery.From.Username is null");

                var crayfishGameProcess = await context.Set<CrayfishGameProcess>().FirstAsync(a => a.ChatId == message.Chat.Id && a.MessageId + 1 == message.MessageId);

                if (data.Contains('*') ||
                    crayfishGameProcess.Winner != null ||
                    (!from.Contains(crayfishGameProcess.FirstUsername) &&
                    !from.Contains(crayfishGameProcess.SecondUsername)))
                    return;

                crayfishGameProcess.Winner = update.CallbackQuery.From.Username;
                var loser = crayfishGameProcess.Winner.Contains(crayfishGameProcess.FirstUsername) ? crayfishGameProcess.SecondUsername : crayfishGameProcess.FirstUsername;

                await botClient.EditMessageTextAsync(
                    new ChatId(crayfishGameProcess.ChatId),
                    (int)crayfishGameProcess.MessageId + 1,
                    $"Победитель @{crayfishGameProcess.Winner} гогочет над @{loser}! СЮДА РАЧОК!");

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
        }
    }
}
