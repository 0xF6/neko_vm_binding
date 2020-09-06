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

        public override string ToString() => Value.ToString();
    }
    public sealed unsafe class NekoInt : NekoObject
    {
        internal NekoInt(NekoValue* value) : base(value) 
            => NekoAssert.IsInt(value);

        public static implicit operator int(NekoInt i)
        {
            if (NekoType.is_null(i.@ref))
                throw new NullReferenceNekoException<int>(i.@ref);
            return ((((int) (IntPtr) (i.@ref)) >> 1));
        }

        public static implicit operator NekoInt(int i) => 
            new NekoInt(Native.neko_alloc_int(i));

        public int Value => this;

        public override string ToString() => Value.ToString();
    }
}