namespace Neko
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Security;
    using Base;
    using NativeRing;
    using static NativeRing.NekoValueType;

    public static unsafe class NekoMarshal
    {
        internal static readonly IDictionary<Type, NekoValueType> variants = new Dictionary<Type, NekoValueType>();

        static NekoMarshal()
        {
            variants.Add(typeof(int), VAL_INT32);
            variants.Add(typeof(string), VAL_STRING);
            variants.Add(typeof(bool), VAL_BOOL);
            variants.Add(typeof(float), VAL_FLOAT);
            variants.Add(typeof(NekoFunction), VAL_FUNCTION);
            variants.Add(typeof(NekoObject), VAL_OBJECT);
        }

        public static T PtrToCLR<T>(NekoValue* value) 
            => (T)CreateInstance(value);

        public static NekoValue* CLRToPrt<T>(T value) 
            => CLRToPrt((object)value);

        public static NekoValue* CLRToPrt(object value)
        {
            if (GetNekoVariant(value) == VAL_FUNCTION)
                return ((NekoFunction) value).@ref;
            if (GetNekoVariant(value) == VAL_STRING)
                return Native.neko_alloc_string((string)value);
            if (GetNekoVariant(value) == VAL_INT32)
                return Native.neko_alloc_int32((int)value);
            if (GetNekoVariant(value) == VAL_BOOL)
                return Native.neko_alloc_bool((bool)value);
            // TODO
            throw new TypeIsNotSupportNekoException($"{value.GetType().Name}");
        }
        public static NekoValueType GetNekoVariant(object o)
        {
            if (variants.ContainsKey(o.GetType()))
                return variants[o.GetType()];
            throw new TypeIsNotSupportNekoException(o.GetType().Name);
        }

        public static Type GetCLRVariant(uint type)
            => GetCLRVariant((NekoValueType) type);
        public static Type GetCLRVariant(NekoValueType type)
        {
            if (variants.Any(x => x.Value == type))
                return variants.First(x => x.Value == type).Key;
            throw new TypeIsNotSupportNekoException($"{type}");
        }

        public static object CreateInstance(NekoValue* value)
        {
            if (NekoType.get_valtype(value) == VAL_FUNCTION)
                return new NekoFunction(NekoString.GetString(((NekoFunction.__function*) value)->env), value);
            if (NekoType.get_valtype(value) == VAL_STRING)
                return NekoString.GetString(value);
            if (NekoType.get_valtype(value) == VAL_INT32)
                return ((vint32*)value)->i;
            if (value == Native.v_true())
                return true;
            if (value == Native.v_false())
                return false;
            // TODO
            throw new TypeIsNotSupportNekoException($"{(NekoValueType)value->t}");
        }
    }
}
