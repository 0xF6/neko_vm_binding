namespace Neko.Base
{
    using System;
    using NativeRing;
    public sealed unsafe class NekoInt : NekoObject
    {
        internal NekoInt(NekoValue* value) : base(value)
            => NekoAssert.IsInt(value);

        public static implicit operator int(NekoInt i)
        {
            if (NekoType.is_null(i.@ref))
                throw new NullReferenceNekoException<int>(i.@ref);
            return ((((int)(IntPtr)(i.@ref)) >> 1));
        }

        public static implicit operator NekoInt(int i) =>
            new NekoInt(Native.neko_alloc_int(i));

        public int Value => this;

        public override string ToString() => Value.ToString();
    }
}