namespace Neko.Base
{
    using NativeRing;

    public sealed unsafe class NekoArray : NekoObject
    {
        internal NekoArray(NekoValue* value) : base(value) => 
            NekoAssert.IsArray(value);


        public __array* AsInternal() => (__array*)@ref;

        public struct __array
        {
            public uint t;
            public NekoValue* ptr;
        }
    }
}