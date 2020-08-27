namespace nekoc
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;
    using Pastel;
    using static System.Console;
    using static System.Environment;
    using static System.Environment.SpecialFolder;
    using static System.IO.Path;

    internal class Program
    {
        public const string AssemblyKey = "nekoc";
        public const string CompilersFolder = "resources.binaries";

        public static string GetPathCompilerForCurrentOS()
        {
            var result = $"{AssemblyKey}.{CompilersFolder}.{GetOS()}_x64.nekoc";
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
            new(Combine(Combine(GetFolderPath(ApplicationData), "dotnet-nekoc-cache"), GetVersion()));

        public static FileInfo GetCompilerBinariesFile()
        {
            var cache = GetFolderForCache();
            if (!cache.Exists)
                Directory.CreateDirectory(cache.FullName);
            return new(Combine(cache.FullName, $"nekoc{(GetOS() == "win" ? ".exe" : "")}"));
        }

        public static async Task<FileInfo> GetOrCreateCompilerAsync()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                OutputEncoding = Encoding.Unicode;

            PrintHeader();
            if (GetOS() != "osx")
            {
                WriteLine($"Please note that the tool may not work correctly under the OSX operating system.".Pastel(Color.Orange));
                WriteLine($"If you find a problem, report it to the repository.".Pastel(Color.Orange));
            }

            var result = GetCompilerBinariesFile();

            if (result.Exists)
                return result;

            await using var stream = GetCompilerContent();
            await using var memory = new MemoryStream();
            await stream.CopyToAsync(memory);
            await File.WriteAllBytesAsync(result.FullName, memory.ToArray());

            return result;
        }

        private static void PrintHeader()
        {
            if (GetEnvironmentVariable("WT_SESSION") == null && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                SetEnvironmentVariable($"NO_COLOR", "true");
                return;
            }
            WriteLine($"⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿\r\n⣿⣿⣿⠈⠻⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡿⠋⢹⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿\r\n⣿⣿⣿⡀⢳⣤⡙⠻⢿⣿⣿⣿⠿⢿⣿⣿⠿⢿⣿⣿⣿⠿⠛⣡⣴⠇⣼⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿\r\n⣿⣿⣿⣧⠸⡀⡙⢶⣤⣈⣁⣤⣤⠀⠹⠃⢴⣶⣤⣭⣤⡶⢋⡁⡜⢠⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿\r\n⣿⣿⣿⣿⣆⢻⡼⣷⡍⠉⠁⠀⣹⣆⠀⢀⣿⡁⠀⠉⠉⣴⡏⣼⢃⣾⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿\r\n⣿⣿⣿⣿⡿⢀⠇⠉⠀⠀⠀⠘⠛⢻⣦⡾⠛⠋⠀⠀⠀⠈⠁⣧⠸⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿\r\n⣿⣿⣿⡿⢁⡞⠀⠀⣴⡶⠦⣄⡀⠿⣿⡿⠂⣠⠴⠶⣶⡀⠀⠸⣧⢹⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿\r\n⣿⣿⣿⠃⣼⢀⣤⣶⣿⣿⣶⣶⣿⣶⣿⣷⣿⣷⣶⣾⣿⣶⣤⡀⢹⡆⢿⣿⣿⣿⣿⣿⣿⡿⠿⣿⣿⣿⡿⢿⣿⣿⡿⠿⠿⠿⠿⠿⣿⣿⣿⠿⣿⣿⣿⠿⢿⣿⣿⡿⠿⠿⠿⠿⠿⣿⣿⣿⣿⣿\r\n⣿⣿⣿⢰⠃⣠⣿⣿⠿⠿⠟⠿⢿⡇⠀⠀⣹⠿⠛⠛⠻⠿⢿⣇⠈⢷⢸⣿⣿⣿⣿⣿⣿⡇⢠⣄⠙⢿⡇⢸⣿⣿⡗⠒⠒⠒⠒⠒⣿⣿⣿⠀⠛⠛⠃⠐⢻⣿⣿⡇⢰⣶⣶⣶⠀⣿⣿⣿⣿⣿\r\n⣿⣿⡏⢸⡾⠛⢉⣤⣤⣄⠀⠶⠿⠟⠀⠘⠛⠿⠶⢀⣤⣤⣀⠈⠳⣾⠀⣿⣿⣿⣿⣿⣿⣇⣸⣿⣷⣤⣀⣸⣿⣿⣏⣉⣉⣉⣉⣉⣿⣿⣿⣀⣿⣿⣿⣇⣸⣿⣿⣇⣈⣉⣉⣉⣀⣿⣿⣿⣿⣿\r\n⣿⣿⡇⠈⠀⣼⠟⠿⣿⣿⣿⣶⠀⢰⠀⢠⡆⢰⣾⣿⣿⡿⠟⢿⣆⠈⠀⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿\r\n⣿⣿⣇⠀⢰⣏⣴⡾⠟⠛⠛⠛⣇⢸⣿⣿⢇⡟⠛⠛⠛⠿⢷⣄⣿⠀⢀⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿\r\n⣿⣿⣿⣧⠀⠟⠁⣤⣾⣿⣿⣧⠸⣾⣭⣽⣾⢠⣿⣿⣿⣶⣄⠙⠏⢠⣼⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿\r\n⣿⣿⣿⣿⣧⠀⣾⣿⣿⣿⣿⣿⣆⠙⠿⠿⢃⣼⣿⣿⣿⣿⣿⡆⢠⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿\r\n⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣷⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿\r\n⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿"
                .Pastel(Color.MediumPurple));
            WriteLine();
        }

        public static async Task<int> Main(string[] args)
        {
            var result = await GetOrCreateCompilerAsync();

            if (args is { Length: 0 })
                return await HelpAsync(result);
            return await ExecAsync(result, args.Concat(new []{ "-v" }).ToArray());
        }

        private static Task<int> HelpAsync(FileInfo compiler) =>
            ExecAsync(compiler, "--help", "-v");

        public static async Task<int> ExecAsync(FileInfo compiler, params string[] args)
        {
            var inf = new ProcessStartInfo(compiler.FullName, string.Join(' ', args));
            inf.WorkingDirectory = Directory.GetCurrentDirectory();
            var proc = Process.Start(inf);
            await proc.WaitForExitAsync();

            if (proc.ExitCode == 0)
                WriteLine($"Success.".Pastel(Color.GreenYellow));
            return proc.ExitCode;
        }
    }
}
