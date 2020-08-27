namespace nekoc
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using static System.Environment;
    using static System.Environment.SpecialFolder;
    using static System.IO.Path;

    internal class Program
    {
        public const string AssemblyKey = "nekoc";
        public const string CompilersFolder = "resources.binaries";

        public static string GetPathCompilerForCurrentOS()
        {
            var result = $"{AssemblyKey}.{CompilersFolder}.{GetOS()}-x64.nekoc";
            if (GetOS() == "win")
                result = $"{result}.exe";
            return result;
        }

        public static Stream GetCompilerContent() => 
            typeof(Program)
                .GetTypeInfo()
                .Assembly
                .GetManifestResourceStream(GetPathCompilerForCurrentOS());

        private static string GetOS()
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

        public static DirectoryInfo GetFolderForCache() =>
            new(Combine(GetFolderPath(ApplicationData), GetVersion()));

        public static FileInfo GetCompilerBinariesFile()
        {
            var cache = GetFolderForCache();
            if (!cache.Exists)
                Directory.CreateDirectory(cache.FullName);
            return new(Combine(cache.FullName, $"nekoc{(GetOS() == "win" ? ".exe" : "")}"));
        }

        public static async Task<FileInfo> GetOrCreateCompilerAsync()
        {
            var result = GetCompilerBinariesFile();

            if (result.Exists)
                return result;

            await using var stream = GetCompilerContent();
            await using var memory = new MemoryStream();
            await stream.CopyToAsync(memory);
            await File.WriteAllBytesAsync(result.FullName, memory.ToArray());

            return result;
        }

        public static async Task Main(string[] args)
        {
            var result = await GetOrCreateCompilerAsync();

            Console.WriteLine($"Hello, {result.FullName}");
            Console.WriteLine($"{string.Join(' ', args)}");
            Console.WriteLine($"CurrentDirectory: {Directory.GetCurrentDirectory()}");
            Console.WriteLine($"ExecDirectory: {Assembly.GetEntryAssembly().Location}");
        }
    }
}
