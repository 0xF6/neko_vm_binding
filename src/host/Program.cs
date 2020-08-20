namespace neko.host
{
    using System.IO;
    using System.Runtime.InteropServices;
    using Neko;
    using Neko.Base;

    internal class Program
    {
        public static void Main(string[] args)
        {
            using var vm = new Neko();

            var module = vm.LoadModule(new FileInfo("test.n"));
            NekoFunction.Create(module, "log").Invoke();
        }
    }
}
