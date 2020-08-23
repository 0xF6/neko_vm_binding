namespace Neko.NativeRing
{
    using System.Runtime.CompilerServices;
    using static NekoValueType;

    public unsafe struct NekoValue
    {
        public uint t;
        public NekoValueType GetValueType()
        {
            if (NekoType.is_function(Unsafe.AsPointer(ref this)))
                return VAL_FUNCTION;
            if (NekoType.is_array(Unsafe.AsPointer(ref this)))
                return VAL_ARRAY;
            if (NekoType.is_int(Unsafe.AsPointer(ref this)))
                return VAL_INT;
            if (NekoType.is_float(Unsafe.AsPointer(ref this)))
                return VAL_FLOAT;
            if (NekoType.is_int32(Unsafe.AsPointer(ref this)))
                return VAL_INT32;
            if (NekoType.is_object(Unsafe.AsPointer(ref this)))
                return VAL_OBJECT;
            if (NekoType.is_string(Unsafe.AsPointer(ref this)))
                return VAL_STRING;
            return NekoType.get_valtype(Unsafe.AsPointer(ref this));
        }
    }
}