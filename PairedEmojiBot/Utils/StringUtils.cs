namespace PairedEmojiBot.Utils
{
    public static class StringUtils
    {
        public static string GetUserFIOAndUsername(Telegram.Bot.Types.User user)
        {
            return (user.FirstName + " " + user.LastName).Trim() + (user.Username != null ? "(@" + user.Username + ")" : "");
        }

        public static string GetUserFIOAndUsername(Models.User user)
        {
            return (user.FirstName + " " + user.LastName).Trim() + (user.Username != null ? "(@" + user.Username + ")" : "");
        }
    }
}
