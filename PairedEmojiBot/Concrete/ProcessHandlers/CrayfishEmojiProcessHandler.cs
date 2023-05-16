using Microsoft.EntityFrameworkCore;
using PairedEmojiBot.Abstract;
using PairedEmojiBot.Db;
using PairedEmojiBot.Enums;
using PairedEmojiBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace PairedEmojiBot.Concrete.ProcessHandlers
{
    public class CrayfishEmojiProcessHandler : BaseEmojiHandler
    {
        public override string GetEmojiCommand()
        {
            return "crayfishGame";
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
                var from = callbackQuery.From.Username;

                var crayfishGameProcess = await context.Set<CrayfishGameProcess>().FirstAsync(a => a.ChatId == message.Chat.Id && a.MessageId == message.MessageId);

                if (crayfishGameProcess.FirstUsername == from)
                    crayfishGameProcess.ApproveFirstUser = true;
                else if (crayfishGameProcess.SecondUsername == from)
                    crayfishGameProcess.ApproveSecondUser = true;
                else if (crayfishGameProcess.SecondUsername == null && crayfishGameProcess.FirstUsername != from)
                {
                    crayfishGameProcess.SecondUsername = from;
                    crayfishGameProcess.ApproveSecondUser = true;
                }

                if (crayfishGameProcess.ApproveFirstUser && crayfishGameProcess.ApproveSecondUser)
                {
                    await botClient.EditMessageTextAsync(
                        new ChatId(crayfishGameProcess.ChatId),
                        (int)crayfishGameProcess.MessageId,
                        "Все готовы, приготовьтесь!");

                    Thread.Sleep(new Random().Next(1, 6) * 1000);

                    var rnd1 = new Random().Next(1, 4);
                    var rnd2 = new Random().Next(1, 4);

                    var rndShootButtons = new List<List<InlineKeyboardButton>>();

                    for (var i = 1; i <= 3; i++)
                    {
                        var row = new List<InlineKeyboardButton>();
                        for (var j = 1; j <= 3; j++)
                        {
                            if (i == rnd1 && j == rnd2)
                            {
                                row.Add(InlineKeyboardButton.WithCallbackData("+", "BOOM!+"));
                            }
                            else
                            {
                                row.Add(InlineKeyboardButton.WithCallbackData("*", "BOOM!*"));
                            }
                        }
                        rndShootButtons.Add(row);
                    }

                    await botClient.EditMessageTextAsync(
                        new ChatId(crayfishGameProcess.ChatId),
                        (int)crayfishGameProcess.MessageId,
                        EmojiEnum.CRAYFISH_EMOJI);

                    await botClient.EditMessageTextAsync(
                        new ChatId(crayfishGameProcess.ChatId),
                        (int)crayfishGameProcess.MessageId + 1,
                        "А ну ка кто быстрее?",
                        replyMarkup: new InlineKeyboardMarkup(rndShootButtons));
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
