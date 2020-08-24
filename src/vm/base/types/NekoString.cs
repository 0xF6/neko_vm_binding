namespace Neko.Base
{
    using System;
    using System.Runtime.InteropServices;
    using NativeRing;

    public unsafe class NekoString : NekoObject, INativeCast<_neko_string>
    {
        public override string ToString() 
            => Marshal.PtrToStringUTF8((IntPtr)(&((_neko_string*)@ref)->c));

        public static implicit operator string(NekoString str) 
            => str.ToString();
        public static implicit operator NekoString(string str) 
            => new NekoString(Native.neko_alloc_string(str));

        public string Value => ToString();

        public _neko_string* AsInternal() => (_neko_string*) @ref;
        protected internal NekoString(NekoValue* value) : base(value) 
            => NekoAssert.IsString(value);
    }
}