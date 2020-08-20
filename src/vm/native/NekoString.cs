namespace Neko.NativeRing
{
    using System;
    using System.Runtime.InteropServices;

    public unsafe struct NekoString
    {
        public uint t;
        public char c;

        public bool IsNull() => t == default;
        public NekoValueType GetValueType() => (NekoValueType) t;

        public static string GetString(NekoValue* v) 
            => Marshal.PtrToStringUTF8((IntPtr)(&((NekoString*)v)->c));
    }
}