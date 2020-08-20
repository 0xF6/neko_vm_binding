namespace Neko.Base
{
    using System;
    using System.Runtime.InteropServices;
    using NativeRing;
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void static_delegate();
    public sealed unsafe class NekoFunction : NekoObject
    {
        private readonly NekoFunctionKind _kind;
        public string Name { get; }
        public int ArgCount { get; }

        public NekoFunction(string functionName, NekoValue* value) : base(value)
        {
            NekoAssert.IsFunction(value, functionName);
            Name = functionName;
            ArgCount = AsInternal()->nargs;
            _kind = NekoFunctionKind.Imported;
        }

        public static NekoFunction Create(static_delegate actor, string name)
        {
            // TODO check function is static
            var p = Marshal.GetFunctionPointerForDelegate(actor);
            var result = Native.neko_alloc_function((void*)p, 0, name);
            return new NekoFunction(name, result);
        }

        public static NekoFunction Create(NekoModule module, string functionName)
        {
            var field = Native.neko_val_field(module, Native.neko_val_id(functionName));
            return new NekoFunction(functionName, field);
        }
        public object Invoke2(params object[] args)
        {
            if(args.Length != ArgCount)
                throw new Exception();

            return Native.neko_val_call0(this.@ref)->t; // TODO
        }

        public NekoValue* Invoke()
        {
            if (0 != ArgCount)
                throw new Exception();
            return Native.neko_val_call0(this.@ref);
        }
        public NekoValue* Invoke(params NekoValue*[] args)
        {
            if(args.Length != ArgCount)
                throw new Exception();
            if(args.Length == 1)
                return Native.neko_val_call1(this.@ref, args[0]);
            throw new Exception();
        }

        public bool IsExported() => _kind == NekoFunctionKind.Exported;


        internal __function* AsInternal() => (__function*) @ref;
        internal struct __function
        {
            public uint t;
            public int nargs;
            public void* addr;
            public NekoValue* env;
            public void* module;
        }
    }
}