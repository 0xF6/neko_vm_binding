namespace Neko.NativeRing
{
    using System;
    using System.Runtime.InteropServices;

    public unsafe struct NekoString
    {
        public uint t;
        public char c;

        public bool IsNull() => t == default;
        public val_type GetValueType() => (val_type) t;

        public static string GetString(NekoValue* v) 
            => Marshal.PtrToStringUTF8((IntPtr)(&((NekoString*)v)->c));
    }
}