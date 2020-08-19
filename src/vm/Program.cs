namespace Neko
{
    using System;
    using Base;
    using NativeRing;


    internal static class Program
    {
        public static unsafe void Main(string[] args)
        {
            using var vm = new Neko();
            Execute(LoadModule("test.n"));
        }
        private static unsafe void Execute(NekoValue* module)
        {
            var id = Native.neko_val_id("log");
            var f = Native.neko_val_field(module, id);
            Native.neko_val_call0(f);
        }
        private static unsafe NekoValue* LoadModule(string moduleName)
        {
            var args = stackalloc NekoValue*[2];
            var exception = (NekoValue*)null;
            var loader = Native.neko_default_loader(null,0);
            args[0] = Native.neko_alloc_string(moduleName);
            args[1] = loader;
                
            var a1 = Native.neko_val_id("loadmodule");
            var a2 = Native.neko_val_field(loader, a1);
            var result = Native.neko_val_callEx(loader, a2, args,2, ref exception);
            if (exception == null || exception->IsNull()) 
                return result;
            var b = Native.neko_alloc_buffer(null);
            Native.neko_val_buffer(b, exception);
            var raw = Native.neko_buffer_to_string(b);
            Console.WriteLine($"Uncaught exception - {NekoString.GetString(raw)}");
            return result;
        }
    }
}
