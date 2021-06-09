namespace Neko.NativeRing
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using Base;

    internal static unsafe class Native
    {
        private static IntPtr _ref;
        private static bool isDebug;
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
            isDebug = Environment.GetEnvironmentVariable("LD_DEBUG") == "libs";
            if (RuntimeInformation.ProcessArchitecture != Architecture.X64)
                throw new NotSupportedException($"Temporary support only x64 os arch.");
            NativeLibrary.SetDllImportResolver(typeof(Native).Assembly, Resolver);
            __debug_loader(".ctor");
        }

        private static void __debug_loader(string s)
        {
            if (!isDebug)
                return;
            lock (Console.Out)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"neko::native{s}");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        private static IntPtr Resolver(string name, Assembly asm, DllImportSearchPath? search)
        {
            if (name.Equals("neko", StringComparison.InvariantCultureIgnoreCase))
            {
                if (_ref == IntPtr.Zero)
                    return _ref = _resolver(name);
                return _ref;
            }
            return _resolver(name);
        }

        private static string[] _getSearchPath() =>
            new[]
            {
                "./{__formatted_lib_name__}",
                "./runtimes/{__OS__}-x64/native/{__formatted_lib_name__}",
                "./bin64/{__formatted_lib_name__}",
                "./Engine64/{__formatted_lib_name__}",
                "{__formatted_lib_name__}" // maybe needed load it first-shot from [/usr/lib, /windows]?
            };

        private static IntPtr _resolver(string name)
        {
            var paths = _getSearchPath()
                .Select(x => x.Replace("{__formatted_lib_name__}", _format_lib(name)))
                .Select(x => x.Replace("{__OS__}", _get_os()))
                .Pipe(x => __debug_loader($".find_path {x}"))
                .ToArray();
            var targetPath = paths.Select<string, (string path, bool result)>
                    (path => (path, NativeLibrary.TryLoad(path, out _)))
                .Pipe(x => __debug_loader($".try_find in {x.path} -> {x.result}"))
                .Where(x => x.result)
                .FirstOrDefault().path ?? name;
            return NativeLibrary.Load(targetPath);
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
        public static extern NekoValue** neko_alloc_root(int gen);
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
        [DllImport("neko", EntryPoint = "get_value_true")]
        public static extern NekoValue* v_true();
        [DllImport("neko", EntryPoint = "get_value_false")]
        public static extern NekoValue* v_false();
        [DllImport("neko")]
        public static extern NekoValue** get_neko_builtins();



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
        public static NekoValue* neko_alloc_bool(bool b) => b ? v_true() : v_false();
        public static NekoValue* neko_alloc_int(int v) => (NekoValue*)(IntPtr)((v << 1) | 1);

        [DllImport("neko")]
        public static extern NekoValue* neko_alloc_float(double value);

        [DllImport("neko")]
        public static extern NekoValue* neko_alloc_array(uint size);
        public static int neko_val_array_size(NekoArray v)
            => (int)((uint)NekoType.tag(v.@ref) >> 4);
    }


    internal static class __linq
    {
        public static IEnumerable<T> Pipe<T>(this IEnumerable<T> @this, Action<T> selector)
        {
            foreach (var value in @this)
            {
                selector(value);
                yield return value;
            }
        }
    }
}