namespace nekoVM_proxy
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using static val_type;

    internal static class Program
    {
        public static unsafe void Main(string[] args)
        {
            Native.neko_global_init();
            NekoVM* result = Native.neko_vm_alloc(IntPtr.Zero.ToPointer());
            Native.neko_vm_select(result);
            Execute(LoadModule("test.n"));
            Native.neko_global_free();
        }
        private static unsafe void Execute(NekoValue* module)
        {
            var id_x = Native.neko_val_id("x");
            var f_x = Native.neko_val_field(module, id_x);
            Assert.IsTrue(NekoType.is_int(f_x));
            var id = Native.neko_val_id("log");
            var f = Native.neko_val_field(module, id);
            Assert.IsTrue(NekoType.is_function(f));
            Native.neko_val_call0(f);
        }
        private static unsafe NekoValue* LoadModule(string moduleName)
        {
            var args = stackalloc NekoValue*[2];
            var exception = (NekoValue*)null;
            var loader = Native.neko_default_loader(null,0);
            args[0] = Native.neko_alloc_string(moduleName.ToRef());
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

    public static class Assert
    {
        public static void IsTrue(bool v)
        {
            if(!v)
                throw new Exception();
        }
    }
    internal static unsafe class NativeEx
    {
        public static void* ToRef(this string str) => Marshal.StringToHGlobalAnsi(str).ToPointer();
    }
    internal static unsafe class Native
    {
        private static IntPtr _ref;
        static Native()
        {
            if (RuntimeInformation.ProcessArchitecture != Architecture.X64)
                throw new NotSupportedException($"Temporary support only x64 os arch.");
            NativeLibrary.SetDllImportResolver(typeof(Native).Assembly, Resolver);
        }

        private static IntPtr Resolver(string name, Assembly asm, DllImportSearchPath? search)
        {
            return NativeLibrary.Load($"./runtimes/{_get_os()}-x64/native/{_format_lib(name)}");
            // TODO, research alternative method loading library when run program with debugger
            if (Debugger.IsAttached && name == "neko") // wtf
                name = $"./runtimes/{_get_os()}-x64/native/{_format_lib(name)}";
            IntPtr _resolver() => NativeLibrary.Load(@"C:\git\nekoVM-proxy\nekoVM-proxy\bin\Debug\netcoreapp3.1\runtimes\win-x64\native\neko.dll");
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
        [DllImport("neko")]
        public static extern NekoValue* neko_alloc_string(void* chars);
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
    public struct NekoVM { }
    public enum val_type : uint
    {
        VAL_INT			= 0xFF,
        VAL_NULL		= 0,
        VAL_FLOAT		= 1,
        VAL_BOOL		= 2,
        VAL_STRING		= 3,
        VAL_OBJECT		= 4,
        VAL_ARRAY		= 5,
        VAL_FUNCTION	= 6,
        VAL_ABSTRACT	= 7,
        VAL_INT32		= 8,
        VAL_PRIMITIVE	= 6 | 16,
        VAL_JITFUN		= 6 | 32,
        VAL_32_BITS		= 0xFFFFFFFF
    }
    public unsafe struct NekoValue
    {
        public uint t;

        public bool IsNull() => NekoType.get_valtype(Unsafe.AsPointer(ref this)) == VAL_NULL;

        public val_type GetValueType() => (val_type) t;
    }

    public unsafe ref struct NekoType
    {
        private const int NEKO_TAG_BITS = 4;

        public static val_type tag(void* v) 
            => *(val_type*)v;
        public static uint short_tag(void* v) 
            => (uint)tag(v) & ((1<<NEKO_TAG_BITS) - 1);
        public static bool is_int(void* v) 
            => ((long)(IntPtr)v & 1) != 0;
        public static bool is_float(void* v) 
            => (!is_int(v) && tag(v) == VAL_FLOAT);
        public static bool is_int32(void* v) 
            => (!is_int(v) && tag(v) == VAL_INT32);
        public static bool is_object(void* v) 
            => (!is_int(v) && tag(v) == VAL_OBJECT);
        public static bool is_abstract(void* v) 
            => (!is_int(v) && tag(v) == VAL_ABSTRACT);
        public static bool is_string(void* v) 
            => (!is_int(v) && short_tag(v) == (uint)VAL_STRING);
        public static bool is_function(void* v) 
            => (!is_int(v) && short_tag(v) == (uint)VAL_FUNCTION);
        public static bool is_array(void* v) 
            => (!is_int(v) && short_tag(v) == (uint)VAL_ARRAY);
        public static val_type get_valtype(void* v) 
            => (val_type)(is_int(v) ? (uint)VAL_INT : short_tag(v));
    }
    public unsafe struct NekoString
    {
        public uint t;
        public char c;

        public bool IsNull() => t == default;
        public val_type GetValueType() => (val_type) t;

        public static string GetString(NekoValue* v) 
            => Marshal.PtrToStringUTF8((IntPtr)(&((NekoString*)v)->c));
    }

    /*
 typedef struct {
	val_type t;
	char c;
} vstring;
     */

    public struct NekoBuffer
    { }

    public unsafe struct hcell
    {
        public int hkey;
        public NekoValue key;
        public NekoValue val;
        public hcell* next;
    }
    public unsafe struct vhash
    {
        public hcell** cells;
        public int ncells;
        public int nitems;
    }
}
