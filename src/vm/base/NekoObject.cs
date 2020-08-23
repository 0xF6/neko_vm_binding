namespace Neko.Base
{
    using System;
    using NativeRing;

    public unsafe class NekoObject : NekoBehaviour
    {
        protected NekoObject(NekoValue* value) : base(value) { }
        public override int GetHashCode() => Native.neko_val_hash(this.@ref);
        public static NekoObject Create(NekoValue* value)
        {
            if (NekoType.is_null(value))
                return new NekoNull(value);
            if (NekoType.is_string(value))
                return new NekoString(value);
            if (NekoType.is_float(value))
                return new NekoFloat(value);
            if (NekoType.is_float(value))
                return new NekoFloat(value);
            if (NekoType.is_array(value))
                return new NekoArray(value);
            if (NekoType.is_int32(value))
                return new NekoInt32(value);
            if (NekoType.is_object(value))
                return new NekoRuntimeObject(value);
            return new NekoObject(value);
        }

        public static implicit operator NekoObject(NekoValue* val) => Create(val);
    }
}