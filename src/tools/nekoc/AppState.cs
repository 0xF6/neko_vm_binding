namespace nekoc
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using static System.Environment.SpecialFolder;
    using static System.Environment;
    using static System.IO.Path;
    public class AppState
    {
        public static bool isTrace = false;
        public static bool forceInstall = false;


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
    }
}