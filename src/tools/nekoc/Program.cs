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
using Microsoft.Extensions.Logging;
using MoreLinq;
using nekoc;
using static System.Console;
using static System.Environment;
using static System.Environment.SpecialFolder;
using static System.IO.Path;
using static nekoc.AppState;

const string NEKO_C_VERSION = "2.3.0";

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    OutputEncoding = Encoding.Unicode;
SetupLogger();
PopArgs(ref args, "--trace", ref isTrace);
PopArgs(ref args, "--force-install", ref forceInstall);

if(isTrace)
    trace("trace log enabled");
if(forceInstall)
    trace("force installing enabled");

PrintHeader();

if (GetOS() != "win")
{
    WriteLine($"Temporarily this tool can only run on the Windows operating system.".Pastel(Color.Orange));
    WriteLine($"Due to some problems when working with tar archives".Pastel(Color.Orange));
    return -1;
}

if (GetOS() == "osx")
{
    WriteLine($"Please note that the tool may not work correctly under the OSX operating system.".Pastel(Color.Orange));
    WriteLine($"If you find a problem, report it to the repository.".Pastel(Color.Orange));
}

var compilerFile = await GetOrCreateCompilerAsync();

trace($"call {nameof(GetOrCreateCompilerAsync)} -> {compilerFile}");

if (args is { Length: 0 })
    return await ExecAsync(compilerFile, "--help", "-v");
return await ExecAsync(compilerFile, args.Concat(new []{ "-v" }).ToArray());




static FileInfo GetCompilerBinariesFile()
{
    var cache = GetFolderForCache();
    if (!cache.Exists)
        Directory.CreateDirectory(cache.FullName);
    return new(Combine(cache.FullName, $"nekoc{(GetOS() == "win" ? ".exe" : "")}"));
}

async Task<FileInfo> GetOrCreateCompilerAsync()
{
    var cache = GetFolderForCache();
    if (forceInstall && cache.Exists)
    {
        cache.EnumerateFiles("*.*").Pipe(x => x.Delete()).Pipe(x => trace($"delete '{x}'..")).Consume();
        cache.EnumerateDirectories().Pipe(x => x.Delete()).Pipe(x => trace($"delete '{x}'..")).Consume();
    }
    var result = GetCompilerBinariesFile();

    if (result.Exists)
        return result;
    Action<string> extractor = null;
    var zip = await new GithubClient("HaxeFoundation", "neko", NEKO_C_VERSION).DownloadAsync();
    if (GetOS() == "win")
        extractor = (s) => ZipFile.ExtractToDirectory(zip.FullName, s);
    else
        throw new NotSupportedException(); // fucking tar archive and fucking poxis pex

    extractor(GetFolderForCache().FullName);

    var targetDir = GetFileNameWithoutExtension(zip.FullName);
    var extractedDir = Combine(cache.FullName, targetDir);

    foreach (var file in Directory.GetFiles(Combine(cache.FullName, targetDir), "*.*", SearchOption.AllDirectories))
        File.Move(file, Combine(cache.FullName, GetFileName(file)));
    Directory.Delete(extractedDir, true);
    return result;
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
    var proc = Process.Start(inf);
    await proc.WaitForExitAsync();
    trace($"process '{compiler}' has complete execution with {proc.ExitCode} exit code");
    if (proc.ExitCode == 0)
        WriteLine($"success".Pastel(Color.GreenYellow));
    return proc.ExitCode;
}

void trace(string s) => logger.LogTrace(s);