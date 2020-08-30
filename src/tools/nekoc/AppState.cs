namespace nekoc
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Console;
    using static System.Environment.SpecialFolder;
    using static System.Environment;
    using static System.IO.Path;
    public class AppState
    {
        public static bool isTrace = false;
        public static bool forceInstall = false;
        public static ILogger<AppState> logger { get; private set; }


        public static DirectoryInfo GetFolderForCache() =>
            new(Combine(GetFolderPath(ApplicationData), "dotnet-nekoc-cache", GetVersion()));

        public static string GetOS()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return "win";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return "linux";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return "osx";
            throw new NotSupportedException();
        }

        public static string GetVersion() =>
            Assembly.GetEntryAssembly()?
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion ?? "any";


        public static void SetupLogger() =>
            logger = LoggerFactory.Create(x => x
                .AddConsoleFormatter<NekoConsoleFormatter, ConsoleFormatterOptions>()
                .AddConsole(x => x.FormatterName = nameof(NekoConsoleFormatter))).CreateLogger<AppState>();

        private ILoggerFactory factory;
    }
}