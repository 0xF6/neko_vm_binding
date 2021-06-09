namespace Neko.Base
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Dynamic;
    using NativeRing;

    public sealed unsafe class NekoModule : NekoBehaviour
    {
        private DynamicNekoModuleProxy proxy { get; set; }

        internal NekoModule(NekoValue* value) : base(value)
            => proxy = new DynamicNekoModuleProxy(this);

        public NekoFunction this[string name] => NekoFunction.Create(this, name);


        public dynamic AsDynamic() => proxy;

        public __module* AsInternal() => (__module*)@ref;

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

        public class DynamicNekoModuleProxy : DynamicObject
        {
            private readonly NekoModule _obj;

            internal DynamicNekoModuleProxy(NekoModule obj) => _obj = obj;



            public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
            {
                try
                {
                    result = _obj[binder.Name].Invoke(args);
                    return true;
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);
                    result = null;
                    return false;
                }
            }
        }
    }
}