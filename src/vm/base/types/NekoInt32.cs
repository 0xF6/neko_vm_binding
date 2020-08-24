namespace Neko.Base
{
    using System;
    using NativeRing;

    public sealed unsafe class NekoInt32 : NekoObject, INativeCast<_neko_int32>
    {
        internal NekoInt32(NekoValue* value) : base(value) 
            => NekoAssert.IsInt32(value);
        public _neko_int32* AsInternal() => (_neko_int32*) @ref;


        public static implicit operator int(NekoInt32 i)
        {
            if (NekoType.is_null(i.@ref))
                throw new NullReferenceNekoException<int>(i.@ref);
            return i.AsInternal()->i;
        }

        public static implicit operator NekoInt32(int i) => 
            new NekoInt32(Native.neko_alloc_int32(i));

        public int Value => this;
    }
}