namespace nekoc
{
    using System;

    public static class StringEx
    {
        public static string Emoji(this string str)
        {
            if (Environment.GetEnvironmentVariable("EMOJI_USE") == "0")
                return "";
            return EmojiOne.EmojiOne.ShortnameToUnicode(str);
        }
    }
}