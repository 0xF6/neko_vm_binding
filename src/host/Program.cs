namespace neko.host
{
    using System.Runtime.InteropServices;
    using nekoVM_proxy;

    internal class Program
    {
        public static unsafe void Main(string[] args)
        {
            NativeLibrary
            using var vm = new Neko();
            Execute(LoadModule("test.n"));
        }
    }
}
