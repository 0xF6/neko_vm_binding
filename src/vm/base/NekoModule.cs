namespace Neko.Base
{
    using NativeRing;

    public sealed unsafe class NekoModule : NekoBehaviour
    {
        internal NekoModule(NekoValue* value) : base(value) { }

        public NekoFunction this[string name] => NekoFunction.Create(this, name);

        public __module* AsInternal() => (__module*) @ref;

        public struct __module
        {
            public void* jit;
            public uint nglobals;
            public uint nfields;
            public uint codesize;
            public NekoValue* name;
            public NekoValue** globals;
            public NekoValue** fields;
            public NekoValue* loader;
            public NekoValue* exports;
            public NekoValue* jit_gc;
            public NekoValue* dbgtbl;
            public NekoDebug* dbgidxs;
            public void* code;
        }

        public struct NekoDebug
        {
            public int @base;
            public uint bits;
        }
    }
}