namespace Neko.Base
{
    using System;
    using NativeRing;

    public sealed unsafe class NekoRuntimeException : NekoObject, INativeCast<__exception>
    {
        public string Message { get; }
        public int Line { get; }
        public string File { get; }
        public string Function { get; }
        internal NekoRuntimeException(NekoValue* value) : base(value)
        {
            __exception* raw = AsInternal();

            Message = new NekoString((NekoValue*)raw->msg);
            File = new NekoString((NekoValue*)raw->file);
            Function  = new NekoString((NekoValue*)raw->funcsig);
            Line = ((IntPtr)(raw->line)).ToInt32();
        }


        
        public __exception* AsInternal() => (__exception*) this.@ref;
    }
}