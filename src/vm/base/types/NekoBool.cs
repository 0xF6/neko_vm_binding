namespace Neko.Base
{
    using NativeRing;

    public sealed unsafe class NekoBool : NekoObject
    {
        internal NekoBool(NekoValue* value) : base(value) => NekoAssert.IsBool(value);


        public bool Value => this.@ref == Native.v_true();

        public static implicit operator bool(NekoBool v) => v.Value;
        public static implicit operator NekoBool(bool v) => v ? new NekoBool(Native.v_true()) : new NekoBool(Native.v_false());
    }
}