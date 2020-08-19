namespace Neko
{
    using System;

    public unsafe ref struct NekoType
    {
        private const int NEKO_TAG_BITS = 4;

        public static val_type tag(void* v) 
            => *(val_type*)v;
        public static uint short_tag(void* v) 
            => (uint)tag(v) & ((1<<NEKO_TAG_BITS) - 1);
        public static bool is_int(void* v) 
            => ((long)(IntPtr)v & 1) != 0;
        public static bool is_float(void* v) 
            => (!is_int(v) && tag(v) == val_type.VAL_FLOAT);
        public static bool is_int32(void* v) 
            => (!is_int(v) && tag(v) == val_type.VAL_INT32);
        public static bool is_object(void* v) 
            => (!is_int(v) && tag(v) == val_type.VAL_OBJECT);
        public static bool is_abstract(void* v) 
            => (!is_int(v) && tag(v) == val_type.VAL_ABSTRACT);
        public static bool is_string(void* v) 
            => (!is_int(v) && short_tag(v) == (uint)val_type.VAL_STRING);
        public static bool is_function(void* v) 
            => (!is_int(v) && short_tag(v) == (uint)val_type.VAL_FUNCTION);
        public static bool is_array(void* v) 
            => (!is_int(v) && short_tag(v) == (uint)val_type.VAL_ARRAY);
        public static val_type get_valtype(void* v) 
            => (val_type)(is_int(v) ? (uint)val_type.VAL_INT : short_tag(v));
    }
}