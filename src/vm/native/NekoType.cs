﻿namespace Neko.NativeRing
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Base;

    public unsafe ref struct NekoType
    {
        private const int NEKO_TAG_BITS = 4;



        public static NekoValueType tag(void* v)
        {
            if (v == null)
                return NekoValueType.VAL_NULL;
            return *(NekoValueType*) v;
        }

        public static uint short_tag(void* v) 
            => (uint)tag(v) & ((1<<NEKO_TAG_BITS) - 1);
        public static bool is_int(void* v) 
            => ((long)(IntPtr)v & 1) != 0;
        public static bool is_float(void* v) 
            => (!is_int(v) && tag(v) == NekoValueType.VAL_FLOAT);
        public static bool is_int32(void* v) 
            => (!is_int(v) && tag(v) == NekoValueType.VAL_INT32);
        public static bool is_object(void* v) 
            => (!is_int(v) && tag(v) == NekoValueType.VAL_OBJECT);
        public static bool is_abstract(void* v) 
            => (!is_int(v) && tag(v) == NekoValueType.VAL_ABSTRACT);
        public static bool is_string(void* v) 
            => (!is_int(v) && short_tag(v) == (uint)NekoValueType.VAL_STRING);
        public static bool is_function(void* v) 
            => (!is_int(v) && short_tag(v) == (uint)NekoValueType.VAL_FUNCTION);
        public static bool is_array(void* v) 
            => (!is_int(v) && short_tag(v) == (uint)NekoValueType.VAL_ARRAY);
        public static bool is_boolean(void* v) 
            => (tag(v) == NekoValueType.VAL_BOOL);

        public static bool is_null(void* v)
            => get_valtype(v) == NekoValueType.VAL_NULL;
        public static NekoValueType get_valtype(void* v) 
            => (NekoValueType)(is_int(v) ? (uint)NekoValueType.VAL_INT : short_tag(v));


        public static bool IsCompatible(ParameterInfo t) 
            => IsCompatible(t.ParameterType);
        public static bool IsCompatible(Type t)
        {
            if (new [] { typeof(void*), typeof(nint), typeof(nuint) }.Any(x => x == t))
                return true;
            if (t == typeof(NekoObject))
                return true;
            return t.IsSubclassOf(typeof(NekoBehaviour));
        }
    }
}