namespace Neko.Base
{
    using System;
    using NativeRing;
    public unsafe delegate void Extract(NekoValue* data, int id, NekoValue** _);
    public sealed unsafe class NekoRuntimeObject : NekoObject, INativeCast<_runtime_obj>
    {
        internal NekoRuntimeObject(NekoValue* value) : base(value) 
            => NekoAssert.IsRuntimeObject(value);

        public int FieldsCount => (&AsInternal()->table)->count;
        public NekoValue* GetFields()
        {
            var array = new NekoArray(Native.neko_alloc_array((uint)FieldsCount));
            Iterator(&AsInternal()->table, rec, &array.AsInternal()->ptr);
            return array;
        }
        public static void rec(NekoValue* d, int id, void**array)
        {
            **array++ = ((IntPtr)id);
        }

        public void Iterator(_neko_objtable *t, Extract extractor, NekoValue** p) 
        {
            var c = t->cells;
            for (var i = 0; i < t->count; i++)
                extractor(c[i].v, c[i].id, p);
        }
        public _runtime_obj* AsInternal() => (_runtime_obj*) @ref;
    }
}