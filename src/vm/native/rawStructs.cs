namespace Neko.NativeRing
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public unsafe struct _neko_cell
    {
        public int hkey;
        public NekoValue key;
        public NekoValue val;
        public _neko_cell* next;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public unsafe struct _neko_hash
    {
        public _neko_cell** cells;
        public int ncells;
        public int nitems;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct _neko_int32
    {
        public uint t;
        public int i;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct _neko_float
    {
        public uint t;
        public double f;
    }

    public unsafe struct _neko_array
    {
        public uint t;
        public NekoValue* ptr;
    }

    public unsafe struct _neko_string
    {
        public uint t;
        public char c;
    }
    public unsafe struct _neko_function
    {
        public uint t;
        public int nargs;
        public void* addr;
        public NekoValue* env;
        public void* module;
    }

    public unsafe struct _neko_vm
    {
        public IntPtr sp;
        public IntPtr csp;
        public NekoValue* env;
        public NekoValue* vthis;
        public IntPtr spmin;
        public IntPtr spmax;
        public IntPtr trap;
        public void* jit_val;
        public void* jmp_buf;
        public int run_jit;
        public NekoValue* exc_stack;
        public void* print;
        public void* print_param;
        public void* clist;
        public NekoValue* resolver;
        public fixed char tmp[100];
        public int trusted_code;
    }

    public unsafe struct _neko_objcell
    {
        public int id;
        public NekoValue* v;
    }
    public unsafe struct _neko_objtable
    {
        public int count;
        public _neko_objcell* cells;
    }
    public unsafe struct _runtime_obj
    {
        public uint t;
        public _neko_objtable table;
        public void* proto;
    }

    public unsafe struct __exception
    {
        public uint t;
        public _neko_string* msg;
        public nint line;
        public _neko_string* file;
        public _neko_string* funcsig;
    }

}