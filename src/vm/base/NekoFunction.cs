namespace Neko.Base
{
    using System;
    using Base;
    using NativeRing;

    public sealed unsafe class NekoFunction : NekoObject
    {
        private readonly NekoModule _module;
        private readonly string _functionName;
        private readonly int _argCount;
        public NekoFunction(NekoModule module, string functionName, NekoValue* value) : base(value)
        {
            NekoAssert.IsFunction(value, functionName);
            _module = module;
            _functionName = functionName;
            _argCount = AsInternal()->nargs;
        }

        public static NekoFunction Create(NekoModule module, string functionName)
        {
            var field = Native.neko_val_field(module, Native.neko_val_id(functionName));
            return new NekoFunction(module, functionName, field);
        }

        public object Invoke(params object[] args)
        {
            if(args.Length != _argCount)
                throw new Exception();

            return Native.neko_val_call0(this.@ref)->t; // TODO
        }


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