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

                        foreach(var user in emojiStatistic.Users)
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
