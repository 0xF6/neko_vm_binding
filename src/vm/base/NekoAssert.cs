namespace Neko.Base
{
    using NativeRing;
    using static NativeRing.NekoValueType;

    internal static unsafe class NekoAssert
    {
        public static void IsFunction(NekoValue* value)
        {
            if (!NekoType.is_function(value))
                throw new InvalidTypeNekoException(VAL_FUNCTION, value);
        }
        public static void IsArray(NekoValue* value)
        {
            if (!NekoType.is_array(value))
                throw new InvalidTypeNekoException(VAL_ARRAY, value);
        }

        public static void IsString(NekoValue* value)
        {
            if (!NekoType.is_string(value))
                throw new InvalidTypeNekoException(VAL_STRING, value);
        }

        public static void IsFloat(NekoValue* value)
        {
            if (!NekoType.is_float(value))
                throw new InvalidTypeNekoException(VAL_FLOAT, value);
        }
        public static void IsInt32(NekoValue* value)
        {
            if (!NekoType.is_int32(value))
                throw new InvalidTypeNekoException(VAL_INT32, value);
        }
        public static void IsNull(NekoValue* value)
        {
            if (!NekoType.is_null(value))
                throw new InvalidTypeNekoException(VAL_NULL, value);
        }

        public static void IsRuntimeObject(NekoValue* value)
        {
            if (!NekoType.is_object(value))
                throw new InvalidTypeNekoException(VAL_OBJECT, value);
        }

        public static void IsBool(NekoValue* value)
        {
            if (!NekoType.is_boolean(value))
                throw new InvalidTypeNekoException(VAL_BOOL, value);
        }
    }
}