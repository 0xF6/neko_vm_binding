namespace Neko.NativeRing
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using Base;

    internal static unsafe class Native
    {
        private static IntPtr _ref;

        private static NekoValue Null = new NekoValue { t = (uint)NekoValueType.VAL_NULL };

        internal static IntPtr libRef
        {
            get
            {
                if (_ref == IntPtr.Zero)
                    return (_ref = NativeLibrary.Load("neko"));
                return _ref;
            }
            set => _ref = value;
        }
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

        public static NekoValue* v_null()
        {
            fixed (NekoValue* p = &Null)
                return p;
        }
        // ((addr*){"val_true"}) + 0x6 - wtf
        public static NekoValue* v_true() => 
            (NekoValue*) NativeLibrary.GetExport((IntPtr)(void*)libRef, "val_true") + 0x6;
        // ((addr*){"val_false"}) + 0x5 - wtf
        public static NekoValue* v_false() => 
            (NekoValue*) NativeLibrary.GetExport((IntPtr)(void*)libRef, "val_false") + 0x5;


        [DllImport("neko")]
        public static extern NekoValue* neko_val_call0(NekoValue* function);
        [DllImport("neko")]
        public static extern NekoValue* neko_val_call1(NekoValue* function, NekoValue* v1);
        [DllImport("neko")]
        public static extern NekoValue* neko_val_call2(NekoValue* function, NekoValue* v1, NekoValue* v2);
        [DllImport("neko")]
        public static extern NekoValue* neko_val_call3(NekoValue* function, NekoValue* v1, NekoValue* v2, NekoValue* v3);
        [DllImport("neko")]
        public static extern NekoValue* neko_val_callN(NekoValue* function, NekoValue** args, int nargs);

        [DllImport("neko")]
        public static extern int neko_val_hash(NekoValue* value);
        [DllImport("neko")]
        public static extern NekoValue* neko_val_field_name(int field);


        [DllImport("neko")]
        public static extern NekoValue* neko_alloc_object(NekoValue* value);
        [DllImport("neko")]
        public static extern void neko_alloc_field(NekoValue* obj, int f, NekoValue* value);
        [DllImport("neko", CharSet = CharSet.Ansi)]
        public static extern NekoValue* neko_alloc_function(void* c_prim, uint args, string name);
        [DllImport("neko")]
        public static extern NekoValue* neko_alloc_int32(int v);


        public static NekoValue* neko_alloc_bool(bool b) => b ? v_true() : v_false();
        public static NekoValue* neko_alloc_int(int v) => (NekoValue*) (IntPtr) ((v << 1) | 1);

        [DllImport("neko")]
        public static extern NekoValue* neko_alloc_float(double value);

        [DllImport("neko")]
        public static extern NekoValue* neko_alloc_array(uint size);
        public static int neko_val_array_size(NekoArray v) 
            => (int)((uint) NekoType.tag(v.@ref) >> 4);
    }
}