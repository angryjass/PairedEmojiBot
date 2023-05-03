using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using PairedEmojiBot.Enums;
using Microsoft.Data.Sqlite;
using PairedEmojiBot.Models;
using PairedEmojiBot.Db;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System;
using System.Linq;

namespace PairedEmojiBot
{
    public static class MainClass
    {
        public static ITelegramBotClient bot = new TelegramBotClient("6188967714:AAEGQ_nMVjW0U82du0yRhGN3dMjPMpH3rwM");
        public static InlineKeyboardMarkup getFive = new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("Дать пять", "Дал пять"));
        private static PairedEmojiBotContext _context = new PairedEmojiBotContext();

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            //TODO АВП перестрелка на скорость реакции
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = update.Message;
                if (message.Text != null && (message.Text.ToLower() == "/five" || message.Text.ToLower() == "/five@paired_emoji_bot"))
                {
                    try
                    {
                        await botClient.SendTextMessageAsync(message.Chat, EmojiEnum.FIVE_EMOJI, replyMarkup: getFive);

                        var outputMessage = await botClient.SendTextMessageAsync(message.Chat, "Количество пятюнь: 0");

                        await _context.AddAsync(
                            new EmojiStatistic()
                            {
                                ChatId = outputMessage.Chat.Id,
                                MessageId = outputMessage.MessageId,
                                Count = 0,
                                Created = DateTime.UtcNow
                            });

                        await _context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }

                    return;
                }
                else if (message.Text != null && (message.Text.ToLower() == "/handshake" || message.Text.ToLower() == "/handshake@paired_emoji_bot"))
                {
                    try
                    {
                        InlineKeyboardMarkup getHandShake = new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("Дать краба", GetUserFIOAndUsername(message.From) + ";" + message.From.Username));
                        await botClient.SendTextMessageAsync(message.Chat, EmojiEnum.REQUEST_HANDSHAKE_EMOJI, replyMarkup: getHandShake);
                        await botClient.SendTextMessageAsync(message.Chat, "И имя красавчика.....");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }

                    return;
                }
                else if (message.Text != null && (message.Text.ToLower().Contains("/crayfish")))
                {
                    try
                    {
                        var arr = message.Text.Split('@');
                        var firstUser = message.From.Username;
                        var secondUser = arr[1].Trim();

                        InlineKeyboardMarkup getApprove = new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("Готов", "crayfishGame"));
                        var outMessage = await botClient.SendTextMessageAsync(message.Chat, $"Господин @{firstUser} пожелал прожарить господина @{secondUser}! Нажмите Готов, чтобы начать дуэль!", replyMarkup: getApprove);
                        await botClient.SendTextMessageAsync(message.Chat, "Ожидание готовности игроков...");

                        await _context.AddAsync(new CrayfishGameProcess()
                        {
                            FirstUsername = firstUser,
                            SecondUsername = secondUser,
                            ChatId = outMessage.Chat.Id,
                            MessageId = outMessage.MessageId,
                        });

                        await _context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
                else if (message.Text != null && (message.Text.ToLower() == "/help" || message.Text.ToLower() == "/help@paired_emoji_bot"))
                {
                    try
                    {
                        await botClient.SendTextMessageAsync(message.Chat, "Инструкция по использованию бота:\n" +
                            $"Отправь /handshake если хочешь, чтобы тебя поддержал твой братишка после прохладной шутки {EmojiEnum.HANDSHAKE_EMOJI}\n" +
                            $"Отправь /five если хочешь собрать пятюни со своих братишек {EmojiEnum.FIVE_EMOJI}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }

                    return;
                }
            }
            else if (update.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                try
                {
                    var message = update.CallbackQuery.Message;
                    if (message.Text == EmojiEnum.REQUEST_HANDSHAKE_EMOJI)
                    {
                        await botClient.EditMessageTextAsync(new ChatId(message.Chat.Id), message.MessageId, EmojiEnum.HANDSHAKE_EMOJI);

                        string initiator = update.CallbackQuery.Data.Split(';')[0];
                        string initiatorUsername = update.CallbackQuery.Data.Split(';')[1];

                        if (initiatorUsername != update.CallbackQuery.From.Username)
                            await botClient.EditMessageTextAsync(new ChatId(message.Chat.Id), message.MessageId + 1, GetUserFIOAndUsername(update.CallbackQuery.From) + " пожал руку " + initiator + ". Ну как же хорош чел");
                        else
                            await botClient.EditMessageTextAsync(new ChatId(message.Chat.Id), message.MessageId + 1, GetUserFIOAndUsername(update.CallbackQuery.From) + " пожал руку сам себе, ну какой же лох");
                    }
                    else if (message.Text == EmojiEnum.FIVE_EMOJI)
                    {
                        var emojiStatistic = await _context.Set<EmojiStatistic>().Include(a => a.Users).FirstAsync(a => a.ChatId == message.Chat.Id && a.MessageId == message.MessageId + 1);
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

                            messageBuilder.AppendLine(GetUserFIOAndUsername(user) + " - " + user.ReactionsCount);
                        }

                        await botClient.EditMessageTextAsync(new ChatId(message.Chat.Id), message.MessageId + 1, messageBuilder.ToString());

                        await botClient.EditMessageTextAsync(new ChatId(message.Chat.Id), message.MessageId, EmojiEnum.OPPOSITE_FIVE_EMOJI, replyMarkup: getFive);
                        Thread.Sleep(700);
                        await botClient.EditMessageTextAsync(new ChatId(message.Chat.Id), message.MessageId, EmojiEnum.FIVE_EMOJI, replyMarkup: getFive);
                        await _context.SaveChangesAsync();
                    }
                    else if (message.ReplyMarkup != null && message.ReplyMarkup.InlineKeyboard.Any() && message.ReplyMarkup.InlineKeyboard.First().First().CallbackData == "crayfishGame")
                    {
                        var from = update.CallbackQuery.From.Username;

                        var crayfishGameProcess = await _context.Set<CrayfishGameProcess>().FirstAsync(a => a.ChatId == message.Chat.Id && a.MessageId == message.MessageId);

                        if (crayfishGameProcess.FirstUsername == from)
                            crayfishGameProcess.ApproveFirstUser = true;
                        else if (crayfishGameProcess.SecondUsername == from)
                            crayfishGameProcess.ApproveSecondUser = true;

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
                    else if (update.CallbackQuery.Data != null && update.CallbackQuery.Data.Contains("BOOM!"))
                    {
                        var crayfishGameProcess = await _context.Set<CrayfishGameProcess>().FirstAsync(a => a.ChatId == message.Chat.Id && a.MessageId + 1 == message.MessageId);

                        if (update.CallbackQuery.Data.Contains("*") ||
                            crayfishGameProcess.Winner != null || 
                            (!update.CallbackQuery.From.Username.Contains(crayfishGameProcess.FirstUsername) && 
                            !update.CallbackQuery.From.Username.Contains(crayfishGameProcess.SecondUsername)))
                            return;

                        crayfishGameProcess.Winner = update.CallbackQuery.From.Username;
                        var loser = crayfishGameProcess.Winner.Contains(crayfishGameProcess.FirstUsername) ? crayfishGameProcess.SecondUsername : crayfishGameProcess.FirstUsername;

                        await botClient.EditMessageTextAsync(
                            new ChatId(crayfishGameProcess.ChatId),
                            (int)crayfishGameProcess.MessageId + 1,
                            $"Победитель @{crayfishGameProcess.Winner} гогочет над @{loser}! СЮДА РАЧОК!");

                        await _context.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

        }

        public static string GetUserFIOAndUsername(Telegram.Bot.Types.User user)
        {
            return (user.FirstName + " " + user.LastName).Trim() + (user.Username != null ? "(@" + user.Username + ")" : "");
        }

        public static string GetUserFIOAndUsername(PairedEmojiBot.Models.User user)
        {
            return (user.FirstName + " " + user.LastName).Trim() + (user.Username != null ? "(@" + user.Username + ")" : "");
        }

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(exception.ToString());
        }
    }
}
