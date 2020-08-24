namespace Neko.Base
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Dynamic;
    using System.Linq;
    using NativeRing;
    public sealed unsafe class NekoRuntimeObject : NekoObject, INativeCast<_runtime_obj>, IEnumerable<NekoObject>
    {
        private DynamicNekoObjectProxy proxy { get; }
        internal NekoRuntimeObject(NekoValue* value) : base(value)
        {
            NekoAssert.IsRuntimeObject(value);
            proxy = new DynamicNekoObjectProxy(this);
        }

        public NekoObject this[string name] 
            => Native.neko_val_field(@ref, Native.neko_val_id(name));

        public dynamic AsDynamic() => proxy;

        public _runtime_obj* AsInternal() => (_runtime_obj*) @ref;

        public string[] GetFields() =>
            (..AsInternal()->table.count)
            .ForEach(x => *&((_runtime_obj*) @ref)->table.cells[x * sizeof(short)])
            .Select(x => (NekoObject)Native.neko_val_field_name(x.id))
            .Cast<NekoString>()
            .Select(x => x.Value)
            .ToArray();



        public class DynamicNekoObjectProxy : DynamicObject
        {
            private readonly NekoRuntimeObject _obj;

            internal DynamicNekoObjectProxy(NekoRuntimeObject obj) => _obj = obj;

            public override IEnumerable<string> GetDynamicMemberNames() => _obj.GetFields();

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                try
                {
                    result = (NekoObject)Native.neko_val_field(_obj.@ref, Native.neko_val_id(binder.Name));
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
        public IEnumerator<NekoObject> GetEnumerator() 
            => GetFields().Select(field => this[field]).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}