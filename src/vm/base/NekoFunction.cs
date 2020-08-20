namespace Neko.Base
{
    using System;
    using NativeRing;

    public sealed unsafe class NekoFunction : NekoObject
    {
        private readonly NekoModule _module;
        private readonly NekoFunctionKind _kind;
        public string Name { get; }
        public int ArgCount { get; }

        public NekoFunction(NekoModule module, string functionName, NekoValue* value) : base(value)
        {
            NekoAssert.IsFunction(value, functionName);
            _module = module;
            Name = functionName;
            ArgCount = AsInternal()->nargs;
            _kind = NekoFunctionKind.Imported;
        }

        public static NekoFunction Create(NekoModule module, string functionName)
        {
            var field = Native.neko_val_field(module, Native.neko_val_id(functionName));
            return new NekoFunction(module, functionName, field);
        }

        public object Invoke(params object[] args)
        {
            if(args.Length != ArgCount)
                throw new Exception();

            return Native.neko_val_call0(this.@ref)->t; // TODO
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