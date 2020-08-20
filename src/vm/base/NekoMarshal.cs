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

    [SecurityCritical]
    public static unsafe class NekoMarshal
    {
        internal static readonly IDictionary<Type, NekoValueType> variants = new Dictionary<Type, NekoValueType>();

        static NekoMarshal()
        {
            variants.Add(typeof(int), VAL_INT32);
            variants.Add(typeof(int), VAL_INT);
            variants.Add(typeof(string), VAL_STRING);
            variants.Add(typeof(bool), VAL_BOOL);
            variants.Add(typeof(float), VAL_FLOAT);
            variants.Add(typeof(NekoFunction), VAL_FUNCTION);
            variants.Add(typeof(NekoObject), VAL_OBJECT);
        }

        public static T PtrToStruct<T>(NekoValue* value) 
            => CreateInstance<T>(value);

        public static NekoValueType GetNekoVariant<T>()
        {
            if (variants.ContainsKey(typeof(T)))
                return variants[typeof(T)];
            throw new TypeIsNotSupportNekoException(typeof(T).Name);
        }

        public static Type GetCLRVariant(uint type)
            => GetCLRVariant((NekoValueType) type);
        public static Type GetCLRVariant(NekoValueType type)
        {
            if (variants.Any(x => x.Value == type))
                return variants.First(x => x.Value == type).Key;
            throw new TypeIsNotSupportNekoException($"{type}");
        }

        public static T CreateInstance<T>(NekoValue* value)
        {
            if (GetNekoVariant<T>() == VAL_FUNCTION)
                throw new Exception("TODO");
            if (GetNekoVariant<T>() == VAL_STRING)
                return (T)(object)NekoString.GetString(value);
            if (value->t == (uint)VAL_INT32)
                return (T) (object) ((int) (IntPtr) value >> 1);
            if (value->t == (uint)VAL_INT32)
                return (T)(object)((vint32*)(value))->i;
            // TODO
            throw new TypeIsNotSupportNekoException($"{(NekoValueType)value->t}");
        }
    }
}
