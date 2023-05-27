using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using PairedEmojiBot.Enums;
using PairedEmojiBot.Db;
using PairedEmojiBot.Concrete.InitHandlers;
using PairedEmojiBot.Concrete.ProcessHandlers;
using PairedEmojiBot.Classifiers;
using PairedEmojiBot.Abstract;

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
                Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));

                var handlers = HandlersCache.HandlersCache.Get(update.Type);

                var classifiersTypes = AppDomain.CurrentDomain.GetAssemblies()
                       .SelectMany(t => t.GetTypes())
                       .Where(t => t.IsClass && t.IsSubclassOf(typeof(BaseClassifier)));

                Delegate? classifiedHandler = null;

                foreach (var classifierType in classifiersTypes) 
                {
                    var classifier = (BaseClassifier)Activator.CreateInstance(classifierType, handlers, update) ?? throw new Exception($"Type {classifierType.Name} is not a BaseClassifier!");
                    classifiedHandler = classifier.Classify();

                    if (classifiedHandler != null) break;
                }

                if (classifiedHandler == null)
                    throw new Exception($"No handler for {update.Type}");

                var task = (Task)classifiedHandler.DynamicInvoke(_context, botClient, update, cancellationToken);

                if (task != null) await task;
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
