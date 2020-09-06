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
            if (NekoType.is_int(value))
                return new NekoInt(value);
            if (NekoType.is_exception(value))
                return new NekoRuntimeException(value);
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
            if (NekoType.is_boolean(value))
                return new NekoBool(value);
            return new NekoObject(value);
        }

        #region implicit operator
        public static implicit operator NekoObject(NekoValue* val) => Create(val);
        public static implicit operator NekoObject(string s) => (NekoString)s;
        public static implicit operator NekoObject(int s) => (NekoInt32)s;
        public static implicit operator NekoObject(bool s) => (NekoBool)s;
        public static implicit operator NekoObject(float s) => (NekoFloat)s;
        public static implicit operator NekoObject(double s) => (NekoFloat)s;

        public static implicit operator string(NekoObject s) => (NekoString)s;
        public static implicit operator int(NekoObject s) => (NekoInt32)s;
        public static implicit operator bool(NekoObject s) => (NekoBool)s;
        public static implicit operator float(NekoObject s) => (NekoFloat)s;
        public static implicit operator double(NekoObject s) => (NekoFloat)s;
        #endregion
        
    }
}