namespace Neko
{
    using System.Runtime.CompilerServices;

    public unsafe struct NekoValue
    {
        public uint t;

        public bool IsNull() 
            => NekoType.get_valtype(Unsafe.AsPointer(ref this)) == val_type.VAL_NULL;

        public val_type GetValueType() => (val_type) t;
    }
}