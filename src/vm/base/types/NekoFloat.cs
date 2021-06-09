namespace Neko.Base
{
    using NativeRing;

    public sealed unsafe class NekoFloat : NekoObject, INativeCast<_neko_float>
    {
        internal NekoFloat(NekoValue* value) : base(value) => NekoAssert.IsFloat(value);
        public _neko_float* AsInternal() => (_neko_float*)@ref;

        public static implicit operator float(NekoFloat i)
        {
            if (NekoType.is_null(i.@ref))
                throw new NullReferenceNekoException<float>(i.@ref);
            return (float)i.AsInternal()->f;
        }
        public static implicit operator double(NekoFloat i)
        {
            if (NekoType.is_null(i.@ref))
                throw new NullReferenceNekoException<double>(i.@ref);
            return i.AsInternal()->f;
        }

        public float GetValue() => this;

        public static implicit operator NekoFloat(float i) =>
            new NekoFloat(Native.neko_alloc_float(i));
        public static implicit operator NekoFloat(double i) =>
            new NekoFloat(Native.neko_alloc_float(i));
    }
}