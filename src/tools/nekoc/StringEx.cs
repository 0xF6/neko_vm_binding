namespace nekoc
{
    using System;
    using System.IO;

    public static class StringEx
    {
        public static string Emoji(this string str) =>
            Environment.GetEnvironmentVariable("EMOJI_USE") == "0"
                ? ""
                : EmojiOne.EmojiOne.ShortnameToUnicode(str);

        public static DirectoryInfo AsDirectoryInfo(this string str) => new (str);
        public static FileInfo AsFileInfo(this string str) => new (str);
    }
}