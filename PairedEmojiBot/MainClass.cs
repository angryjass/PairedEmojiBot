using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using PairedEmojiBot.Enums;
using PairedEmojiBot.Db;
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
            try
            {
                // TODO classifiers
                Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
                if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
                {
                    var message = update.Message;
                    if (message.Text != null && (message.Text.ToLower().Contains("/crayfish")))
                    {
                        var task = (Task)HandlersCache.HandlersCache.Get(update.Type, message.Text.ToLower()).DynamicInvoke(_context, botClient, update, cancellationToken);

                        await task;
                    }
                    else
                    {
                        var task = (Task)HandlersCache.HandlersCache.Get(update.Type, message.Text.ToLower()).DynamicInvoke(_context, botClient, update, cancellationToken);

                        await task;
                    }
                }
                else if (update.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
                {
                    var message = update.CallbackQuery.Message;
                    if (message.ReplyMarkup != null && message.ReplyMarkup.InlineKeyboard.Any() && message.ReplyMarkup.InlineKeyboard.First().First().CallbackData == "crayfishGame")
                    {
                        await new CrayfishEmojiProcessHandler().Handle(_context, botClient, update, cancellationToken);
                    }
                    else if (update.CallbackQuery.Data != null && update.CallbackQuery.Data.Contains("BOOM!"))
                    {
                        await new CrayfishEmojiProcess2Handler().Handle(_context, botClient, update, cancellationToken);
                    }
                    else
                    {
                        var task = (Task)HandlersCache.HandlersCache.Get(update.Type, message.Text.ToLower()).DynamicInvoke(_context, botClient, update, cancellationToken);

                        await task;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(exception.ToString());
        }
    }
}
