namespace Neko.Base
{
    using System;
    using System.Runtime.InteropServices;
    using NativeRing;

    public delegate NekoObject Invoke0();
    public delegate NekoObject InvokeN(params NekoObject[] objs);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void static_delegate();
    public sealed unsafe class NekoFunction : NekoObject, INativeCast<_neko_function>
    {
        private NekoFunctionKind _kind;
        public string Name { get; }
        public int ArgCount { get; }

        public NekoFunction(string functionName, NekoValue* value) : base(value)
        {
            NekoAssert.IsFunction(value);
            Name = functionName;
            ArgCount = AsInternal()->nargs;
            _kind = NekoFunctionKind.Imported;
        }

        public static NekoFunction Create(static_delegate actor, string name)
        {
            // TODO check function is static
            var p = Marshal.GetFunctionPointerForDelegate(actor);
            var result = Native.neko_alloc_function((void*)p, 0, name);
            return new NekoFunction(name, result) {_kind = NekoFunctionKind.Exported};
        }

        public static NekoFunction Create(NekoModule module, string functionName)
        {
            var field = Native.neko_val_field(module, Native.neko_val_id(functionName));
            return new NekoFunction(functionName, field);
        }
        public NekoObject Invoke()
        {
            if (0 != ArgCount)
                throw new Exception();
            return Native.neko_val_call0(@ref);
        }
        public NekoValue* InvokeWithNative(params NekoValue*[] args)
        {
            if(args.Length != ArgCount)
                throw new Exception();
            if(args.Length == 1)
                return Native.neko_val_call1(@ref, args[0]);
            throw new Exception();
        }

        public R Invoke<R>(params object[] args) 
            => NekoMarshal.PtrToCLR<R>(Invoke(args).@ref);

        public NekoObject Invoke(params object[] args)
        {
            if(args.Length != ArgCount)
                throw new InvalidArgumentNekoException();
            var nargs = new NekoValue*[args.Length];
            for (var i = 0; i != args.Length; i++) 
                nargs[i] = NekoMarshal.CLRToPrt(args[i]);
            var result = args.Length switch
            {
                1 => Native.neko_val_call1(@ref, nargs[0]),
                2 => Native.neko_val_call2(@ref, nargs[0], nargs[1]),
                3 => Native.neko_val_call3(@ref, nargs[0], nargs[1], nargs[2]),
                _ => Native.neko_val_callN(@ref, AllocateArgs(args), args.Length)
            };
            return result;
        }

        public bool IsExported() => _kind == NekoFunctionKind.Exported;


        public static NekoValue** AllocateArgs(params object[] args)
        {
            var newArgs = stackalloc NekoValue*[args.Length];
            for (var i = 0; i != args.Length; i++) 
                newArgs[i] = NekoMarshal.CLRToPrt(args[i]);
            return newArgs;
        }

        public static implicit operator Invoke0(NekoFunction fun) => fun.Invoke;
        public static implicit operator InvokeN(NekoFunction fun) => fun.Invoke;


        public _neko_function* AsInternal() => (_neko_function*) @ref;
    }
}