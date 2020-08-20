namespace neko.host
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using Neko;
    using Neko.Base;
    using Neko.NativeRing;

    internal class Program
    {
        public static void tc()
        {
            Console.WriteLine("ITS WORK");
        }
        public static unsafe void Main(string[] args)
        {
            using var vm = new Neko();
            var module = vm.LoadModule(new FileInfo("test.n"));

            var f1 = module["assert_int"].Invoke<int>(1);
            var f2 = module["assert_string"].Invoke<string>("xuy");
            var f3 = module["assert_boolean"].Invoke<bool>(true);
            var f4 = module["assert_function"].Invoke<NekoFunction>(NekoFunction.Create(tc, nameof(tc))).Invoke();
        }
    }
}
