namespace nekoc
{
    using System;

    public static class StringEx
    {
        public static string Emoji(this string str) =>
            Environment.GetEnvironmentVariable("EMOJI_USE") == "0"
                ? ""
                : EmojiOne.EmojiOne.ShortnameToUnicode(str);
    }
}