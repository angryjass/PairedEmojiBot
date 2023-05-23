using System.Reflection;
using Telegram.Bot.Types.Enums;

namespace PairedEmojiBot.HandlersCache
{
    public static class HandlersCache
    {
        private static Dictionary<UpdateType, Dictionary<string, Delegate>> _cache { get; set; } = new Dictionary<UpdateType, Dictionary<string, Delegate>>();

        public static void Add(UpdateType type, string command, Delegate method)
        {
            if (_cache.TryGetValue(type, out var value))
                _cache[type].Add(command, method);
            else
                _cache.Add(type, new Dictionary<string, Delegate>() { { command, method } });
        }

        public static Delegate Get(UpdateType type, string command)
        {
            if (_cache.TryGetValue(type, out var value))
                if (_cache[type].TryGetValue(command, out var method))
                    return method;

            throw new Exception($"No method for type {type} and command {command}");
        }
    }
}
