namespace Neko
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Base;
    using NativeRing;
    using static NativeRing.NekoValueType;

    public static unsafe class NekoMarshal
    {
        internal static readonly IDictionary<Type, NekoValueType> variants = new Dictionary<Type, NekoValueType>();

        static NekoMarshal()
        {
            variants.Add(typeof(int), VAL_INT);
            variants.Add(typeof(string), VAL_STRING);
            variants.Add(typeof(bool), VAL_BOOL);
            variants.Add(typeof(float), VAL_FLOAT);
            variants.Add(typeof(NekoFunction), VAL_FUNCTION);
            variants.Add(typeof(NekoObject), VAL_OBJECT);
        }

        public static T PtrToCLR<T>(NekoValue* value)
        {
            var result = CreateInstance(value);
            if (result is null)
                return default;
            return (T)result;
        }


        public static NekoValue* CLRToPrt<T>(T value) 
            => CLRToPrt((object)value);

        public static NekoValue* CLRToPrt(object value)
        {
            if (value is null)
                return Native.v_null();
            if (value is string s)
                return (NekoString)s;
            if (value is float f)
                return (NekoFloat)f;
            if (value is double d)
                return (NekoFloat)d;
            if (value is bool b)
                return (NekoBool) b;
            if (value is NekoFunction fn)
                return fn.@ref;
            if (value is NekoArray ar)
                return ar.@ref;
            if (value is NekoRuntimeObject ro)
                return ro.@ref;

            #region numbers
            if (value is int i32)
                return (NekoInt)i32;
            if (value is short i16)
                return (NekoInt)i16;
            if (value is ushort u16)
                return (NekoInt)u16;
            if (value is byte u8)
                return (NekoInt)u8;
            if (value is sbyte i8)
                return (NekoInt)i8;
            #endregion
           
            if (value is Delegate)
                throw new NotSupportedException($"temporary delegates not support");
            throw new NotSupportedException($"Type {value.GetType()} is not support marshaling.");
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
            if (NekoType.is_null(value))
                return null;
            if (value == Native.v_true())
                return true;
            if (value == Native.v_false())
                return false;
            if (NekoType.get_valtype(value) == VAL_FUNCTION)
                return new NekoFunction("<unk>", value);
            if (Enum.IsDefined(typeof(NekoValueType), NekoType.get_valtype(value)))
                return NekoObject.Create(value);
            throw new TypeIsNotSupportNekoException($"{(NekoValueType)value->t}");
        }
    }
}
