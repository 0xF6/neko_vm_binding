namespace nekoc
{
    using System;
    using System.Drawing;
    using System.IO;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;
    using Microsoft.Extensions.Logging.Console;
    using Pastel;

    public class NekoConsoleFormatter : ConsoleFormatter
    {
        public NekoConsoleFormatter() : base(nameof(NekoConsoleFormatter)) { }

        public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider scopeProvider, TextWriter textWriter)
        {
            if (logEntry.LogLevel == LogLevel.Trace && !AppState.isTrace)
                return;

            if (logEntry.LogLevel == LogLevel.Critical)
            {
                textWriter.WriteLine($"[{"neko".Pastel(Color.Purple)}][{FormatLevel(logEntry.LogLevel)}]: {logEntry.State.ToString().PastelBg(GetColor(logEntry.LogLevel))}");
                return;
            }
            textWriter.WriteLine($"[{"neko".Pastel(Color.Purple)}][{FormatLevel(logEntry.LogLevel)}]: {logEntry.State}");
        }

        private string FormatLevel(LogLevel level) 
            => $"{$"{level.ToString()[0]}".Pastel(GetColor(level))}";

        private Color GetColor(LogLevel level) =>
            level switch
            {
                LogLevel.Trace => Color.SlateGray,
                LogLevel.Debug => Color.Gray,
                LogLevel.Information => Color.White,
                LogLevel.Warning => Color.Orange,
                LogLevel.Error => Color.Red,
                LogLevel.Critical => Color.Red,
                LogLevel.None => Color.White,
                _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
            };
    }
}