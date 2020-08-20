namespace Neko.NativeRing
{
    using System.Runtime.CompilerServices;

    public unsafe struct NekoValue
    {
        public uint t;

        public bool IsNull() 
            => NekoType.get_valtype(Unsafe.AsPointer(ref this)) == NekoValueType.VAL_NULL;

        public NekoValueType GetValueType() => (NekoValueType) t;
    }
}