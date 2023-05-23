using PairedEmojiBot.Abstract;
using PairedEmojiBot.Db;
using PairedEmojiBot.Enums;
using PairedEmojiBot.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace PairedEmojiBot.Concrete.ProcessHandlers
{
    public class HandshakeEmojiProcessHandler : BaseEmojiHandler
    {
        public override string GetEmojiCommand()
        {
            return EmojiEnum.HANDSHAKE_EMOJI;
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
                await botClient.EditMessageTextAsync(new ChatId(message.Chat.Id), message.MessageId, EmojiEnum.HANDSHAKE_EMOJI);

                string initiator = data.Split(';')[0];
                string initiatorUsername = data.Split(';')[1];

                if (initiatorUsername != update.CallbackQuery.From.Username)
                    await botClient.EditMessageTextAsync(new ChatId(message.Chat.Id), message.MessageId + 1, 
                        StringUtils.GetUserFIOAndUsername(update.CallbackQuery.From) + " пожал руку " + initiator + ". Ну как же хорош чел");
                else
                    await botClient.EditMessageTextAsync(new ChatId(message.Chat.Id), message.MessageId + 1, 
                        StringUtils.GetUserFIOAndUsername(update.CallbackQuery.From) + " пожал руку сам себе, ну какой же лох");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
