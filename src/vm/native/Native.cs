namespace Neko.NativeRing
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.InteropServices;

    internal static unsafe class Native
    {
        internal static IntPtr _ref;
        static Native()
        {
            if (RuntimeInformation.ProcessArchitecture != Architecture.X64)
                throw new NotSupportedException($"Temporary support only x64 os arch.");
            NativeLibrary.SetDllImportResolver(typeof(Native).Assembly, Resolver);
        }

        private static IntPtr Resolver(string name, Assembly asm, DllImportSearchPath? search)
        {
            // TODO, research alternative method loading library when run program with debugger
            if (Debugger.IsAttached && name == "neko") // wtf
                name = $"./runtimes/{_get_os()}-x64/native/{_format_lib(name)}";
            IntPtr _resolver() => NativeLibrary.Load(name);
            if (!name.Equals("neko", StringComparison.InvariantCultureIgnoreCase))
                return _resolver();
            if (_ref == IntPtr.Zero)
                return (_ref = _resolver());
            return _ref;
        }

        private static string _format_lib(string name)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return $"{name}.dll";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return $"lib{name}.so";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return $"{name}.dylib";
            throw new NotSupportedException();
        }
        private static string _get_os()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return "win";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return "linux";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return "osx";
            throw new NotSupportedException();
        }

        [DllImport("neko")]
        public static extern void neko_global_init();
        [DllImport("neko")]
        public static extern void neko_global_free();
        [DllImport("neko")]
        public static extern void neko_gc_major();
        [DllImport("neko")]
        public static extern void neko_gc_loop();
        [DllImport("neko")]
        public static extern NekoVM* neko_vm_alloc(void* _);
        [DllImport("neko")]
        public static extern void neko_vm_select(NekoVM* @ref);
        [DllImport("neko")]
        public static extern NekoValue* neko_default_loader(char** argv, int argc);
        [DllImport("neko")]
        public static extern NekoValue* neko_val_callEx(
            NekoValue* @this, 
            NekoValue* v, 
            NekoValue** args,
            int nargs,
            ref NekoValue* exc);
        [DllImport("neko", CharSet = CharSet.Ansi)]
        public static extern int neko_val_id(string s);
        [DllImport("neko", CharSet = CharSet.Ansi)]
        public static extern NekoValue* neko_alloc_string(string str);
        [DllImport("neko")]
        public static extern NekoValue* neko_val_field(NekoValue* o, int f);
        [DllImport("neko")]
        public static extern NekoBuffer* neko_alloc_buffer(void* chars);
        [DllImport("neko")]
        public static extern void neko_val_buffer(NekoBuffer* buffer, NekoValue* value);
        [DllImport("neko")]
        public static extern NekoValue* neko_buffer_to_string(NekoBuffer* buffer);
        public static NekoValue* val_null() => (NekoValue*)NativeLibrary.GetExport((IntPtr)_ref, "val_null");
        public static NekoValue* val_true() => (NekoValue*)NativeLibrary.GetExport((IntPtr)_ref, "val_true");
        public static NekoValue* val_false() => (NekoValue*)NativeLibrary.GetExport((IntPtr)_ref, "val_false");


        [DllImport("neko")]
        public static extern NekoValue* neko_val_call0(NekoValue* function);
    }
}