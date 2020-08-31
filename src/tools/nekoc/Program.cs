using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Pastel;
using System.IO.Compression;
using Mono.Unix.Native;
using MoreLinq;
using nekoc;
using SharpCompress.Archives.Tar;
using SharpCompress.Readers;
using static System.Console;
using static System.Environment;
using static System.IO.Path;
using static nekoc.AppState;

public static class Input
{
    public static async Task<int> Main(string[] args)
    {
        const string NEKO_C_VERSION = "2.3.0";

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            OutputEncoding = Encoding.Unicode;
        PopArgs(ref args, "--trace", ref isTrace);
        PopArgs(ref args, "--force-install", ref forceInstall);

        if (isTrace)
            trace("trace log enabled");
        if (forceInstall)
            trace("force installing enabled");

        PrintHeader();

        if (GetOS() == "osx")
        {
            warn($"Please note that the tool may not work correctly under the OSX operating system.".Pastel(Color.Orange));
            warn($"If you find a problem, report it to the repository.".Pastel(Color.Orange));
        }

        var compilerFile = await GetOrCreateCompilerAsync();
        
        if (args is { Length: 0 })
            return await ExecAsync(compilerFile, "--help", "-v");
        return await ExecAsync(compilerFile, args.Concat(new[] { "-v" }).ToArray());




        static FileInfo GetCompilerBinariesFile()
        {
            var cache = GetFolderForCache();
            if (!cache.Exists)
                Directory.CreateDirectory(cache.FullName);
            return new(Combine(cache.FullName, $"nekoc{(GetOS() == "win" ? ".exe" : "")}"));
        }

        async Task<FileInfo> GetOrCreateCompilerAsync()
        {
            var extractor = default(Action<string>);
            var cache = GetFolderForCache();
            if (forceInstall && cache.Exists)
            {
                warn($"forceInstall && cache.Exists");
                cache.EnumerateFiles("*.*").Pipe(x => x.Delete()).Pipe(x => trace($"delete '{x}'..")).Consume();
                cache.EnumerateDirectories().Pipe(x => x.Delete()).Pipe(x => trace($"delete '{x}'..")).Consume();
            }
            var c_binary = GetCompilerBinariesFile();
            trace($"lookup compiler binary file...");
            if (c_binary.Exists)
            {
                trace($"exists...");
                return c_binary;
            }
            trace($"not exists... download from github...");
            var zip = await new GithubClient("HaxeFoundation", "neko", NEKO_C_VERSION).DownloadAsync();
            if (GetOS() == "win")
                extractor = (s) => ZipFile.ExtractToDirectory(zip.FullName, s);
            else extractor = (s) =>
            {
                using var file = File.OpenRead(zip.FullName);
                using var tar = TarArchive.Open(file);
                using var reader = tar.ExtractAllEntries();
                trace($"extraction '{zip.FullName}' archive...");
                reader.WriteAllToDirectory(s);

                // Could not write symlink
                // for more information please see https://github.com/dotnet/runtime/issues/24271
                trace($"create 'libneko.so.{NEKO_C_VERSION}'->'libneko.so.{NEKO_C_VERSION[0]}'");
                File.Copy(Combine(s, $"libneko.so.{NEKO_C_VERSION}"), Combine(s, $"libneko.so.{NEKO_C_VERSION[0]}"));
                trace($"create 'libneko.so.{NEKO_C_VERSION}'->'libneko.so'");
                File.Copy(Combine(s, $"libneko.so.{NEKO_C_VERSION}"), Combine(s, $"libneko.so"));
                trace("population binary variation has success");
                trace($"chmod :: {FilePermissions.ACCESSPERMS} -> '{c_binary.FullName}'");
                Syscall.chmod(c_binary.FullName, FilePermissions.ACCESSPERMS);
            };
            extractor(GetFolderForCache().FullName);
            
            return c_binary;
        }

        void PrintHeader()
        {
            if (GetEnvironmentVariable("WT_SESSION") == null && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                trace("disable coloring and emoji...");
                SetEnvironmentVariable($"NO_COLOR", "true");
                SetEnvironmentVariable($"EMOJI_USE", "0");
                return;
            }
            WriteLine($"⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿\r\n⣿⣿⣿⠈⠻⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡿⠋⢹⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿\r\n⣿⣿⣿⡀⢳⣤⡙⠻⢿⣿⣿⣿⠿⢿⣿⣿⠿⢿⣿⣿⣿⠿⠛⣡⣴⠇⣼⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿\r\n⣿⣿⣿⣧⠸⡀⡙⢶⣤⣈⣁⣤⣤⠀⠹⠃⢴⣶⣤⣭⣤⡶⢋⡁⡜⢠⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿\r\n⣿⣿⣿⣿⣆⢻⡼⣷⡍⠉⠁⠀⣹⣆⠀⢀⣿⡁⠀⠉⠉⣴⡏⣼⢃⣾⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿\r\n⣿⣿⣿⣿⡿⢀⠇⠉⠀⠀⠀⠘⠛⢻⣦⡾⠛⠋⠀⠀⠀⠈⠁⣧⠸⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿\r\n⣿⣿⣿⡿⢁⡞⠀⠀⣴⡶⠦⣄⡀⠿⣿⡿⠂⣠⠴⠶⣶⡀⠀⠸⣧⢹⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿\r\n⣿⣿⣿⠃⣼⢀⣤⣶⣿⣿⣶⣶⣿⣶⣿⣷⣿⣷⣶⣾⣿⣶⣤⡀⢹⡆⢿⣿⣿⣿⣿⣿⣿⡿⠿⣿⣿⣿⡿⢿⣿⣿⡿⠿⠿⠿⠿⠿⣿⣿⣿⠿⣿⣿⣿⠿⢿⣿⣿⡿⠿⠿⠿⠿⠿⣿⣿⣿⣿⣿\r\n⣿⣿⣿⢰⠃⣠⣿⣿⠿⠿⠟⠿⢿⡇⠀⠀⣹⠿⠛⠛⠻⠿⢿⣇⠈⢷⢸⣿⣿⣿⣿⣿⣿⡇⢠⣄⠙⢿⡇⢸⣿⣿⡗⠒⠒⠒⠒⠒⣿⣿⣿⠀⠛⠛⠃⠐⢻⣿⣿⡇⢰⣶⣶⣶⠀⣿⣿⣿⣿⣿\r\n⣿⣿⡏⢸⡾⠛⢉⣤⣤⣄⠀⠶⠿⠟⠀⠘⠛⠿⠶⢀⣤⣤⣀⠈⠳⣾⠀⣿⣿⣿⣿⣿⣿⣇⣸⣿⣷⣤⣀⣸⣿⣿⣏⣉⣉⣉⣉⣉⣿⣿⣿⣀⣿⣿⣿⣇⣸⣿⣿⣇⣈⣉⣉⣉⣀⣿⣿⣿⣿⣿\r\n⣿⣿⡇⠈⠀⣼⠟⠿⣿⣿⣿⣶⠀⢰⠀⢠⡆⢰⣾⣿⣿⡿⠟⢿⣆⠈⠀⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿\r\n⣿⣿⣇⠀⢰⣏⣴⡾⠟⠛⠛⠛⣇⢸⣿⣿⢇⡟⠛⠛⠛⠿⢷⣄⣿⠀⢀⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿\r\n⣿⣿⣿⣧⠀⠟⠁⣤⣾⣿⣿⣧⠸⣾⣭⣽⣾⢠⣿⣿⣿⣶⣄⠙⠏⢠⣼⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿\r\n⣿⣿⣿⣿⣧⠀⣾⣿⣿⣿⣿⣿⣆⠙⠿⠿⢃⣼⣿⣿⣿⣿⣿⡆⢠⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿\r\n⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣷⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿\r\n⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿"
                .Pastel(Color.MediumPurple));
            WriteLine();
        }
        static void PopArgs(ref string[] args, string arg, ref bool target)
        {
            if (target = args.Contains(arg))
                args = args.Where(x => x != arg).ToArray();
        }
        async Task<int> ExecAsync(FileInfo compiler, params string[] args)
        {
            trace($"call {nameof(ExecAsync)} -> {compiler} and {string.Join(',', args)}");
            var inf = new ProcessStartInfo(compiler.FullName, string.Join(' ', args));
            inf.WorkingDirectory = Directory.GetCurrentDirectory();
            trace($"working directory '{inf.WorkingDirectory}'");
            var proc = Process.Start(inf);
            await proc.WaitForExitAsync();
            trace($"process '{compiler}' has complete execution with {proc.ExitCode} exit code");
            if (proc.ExitCode == 0)
                trace($"success".Pastel(Color.GreenYellow));
            else
                error($"exit code {proc.ExitCode}".Pastel(Color.Red));
            return proc.ExitCode;
        }

        void trace(string s) => WriteLine($"[{"neko".Pastel(Color.Purple)}][{"T".Pastel(Color.Gray)}]: {s}");
        void error(string s) => WriteLine($"[{"neko".Pastel(Color.Purple)}][{"E".Pastel(Color.Red)}]: {s}");
        void warn(string s) => WriteLine($"[{"neko".Pastel(Color.Purple)}][{"W".Pastel(Color.Orange)}]: {s}");
    }
}