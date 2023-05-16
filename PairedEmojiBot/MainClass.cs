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
using PairedEmojiBot.Utils;
using PairedEmojiBot.Concrete.InitHandlers;
using PairedEmojiBot.Concrete.ProcessHandlers;

namespace PairedEmojiBot
{
    public static class MainClass
    {
        public static ITelegramBotClient bot = new TelegramBotClient("6188967714:AAEGQ_nMVjW0U82du0yRhGN3dMjPMpH3rwM");
        private static InlineKeyboardMarkup getFive = new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("Дать пять", "Дал пять"));
        private static PairedEmojiBotContext _context = new PairedEmojiBotContext();

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = update.Message;
                if (message.Text != null && (message.Text.ToLower() == "/five" || message.Text.ToLower() == "/five@paired_emoji_bot"))
                {
                    await new FiveEmojiInitHandler().Handle(_context, botClient, update, cancellationToken);

                    return;
                }
                else if (message.Text != null && (message.Text.ToLower() == "/handshake" || message.Text.ToLower() == "/handshake@paired_emoji_bot"))
                {
                    await new HandshakeEmojiInitHandler().Handle(_context, botClient, update, cancellationToken);

                    return;
                }
                else if (message.Text != null && (message.Text.ToLower().Contains("/crayfish")))
                {
                    await new CrayfishEmojiInitHandler().Handle(_context, botClient, update, cancellationToken);
                }
                else if (message.Text != null && (message.Text.ToLower() == "/help" || message.Text.ToLower() == "/help@paired_emoji_bot"))
                {
                    await new HelpHandler().Handle(_context, botClient, update, cancellationToken);

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
                        await new HandshakeEmojiProcessHandler().Handle(_context, botClient, update, cancellationToken);
                    }
                    else if (message.Text == EmojiEnum.FIVE_EMOJI)
                    {
                        await new FiveEmojiProcessHandler().Handle(_context, botClient, update, cancellationToken);
                    }
                    else if (message.ReplyMarkup != null && message.ReplyMarkup.InlineKeyboard.Any() && message.ReplyMarkup.InlineKeyboard.First().First().CallbackData == "crayfishGame")
                    {
                        await new CrayfishEmojiProcessHandler().Handle(_context, botClient, update, cancellationToken);
                    }
                    else if (update.CallbackQuery.Data != null && update.CallbackQuery.Data.Contains("BOOM!"))
                    {
                        await new CrayfishEmojiProcess2Handler().Handle(_context, botClient, update, cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

        }

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(exception.ToString());
        }
    }
}
