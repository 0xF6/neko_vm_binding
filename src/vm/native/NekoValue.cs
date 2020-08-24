namespace Neko.NativeRing
{
    using System.Runtime.CompilerServices;
    using static NekoValueType;

    public unsafe struct NekoValue
    {
        public uint t;
        public NekoValueType GetValueType()
        {
            fixed (NekoValue* p = &this)
            {
                if (NekoType.is_function(p))
                    return VAL_FUNCTION;
                if (NekoType.is_array(p))
                    return VAL_ARRAY;
                if (NekoType.is_int(p))
                    return VAL_INT;
                if (NekoType.is_float(p))
                    return VAL_FLOAT;
                if (NekoType.is_int32(p))
                    return VAL_INT32;
                if (NekoType.is_object(p))
                    return VAL_OBJECT;
                return NekoType.is_string(p) ? 
                    VAL_STRING : 
                    NekoType.get_valtype(p);
            }
        }
    }
}