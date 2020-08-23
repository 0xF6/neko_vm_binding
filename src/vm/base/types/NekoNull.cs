namespace Neko.Base
{
    using System;
    using NativeRing;

    public sealed unsafe class NekoNull : NekoObject, IEquatable<NekoNull>
    {
        internal NekoNull(NekoValue* value) : base(value) => NekoAssert.IsNull(value);


        #region Equality members

        public bool Equals(NekoNull other) => true;
        public override bool Equals(object obj)
        {
            if (obj is null)
                return true;
            return ReferenceEquals(this, obj) || obj is NekoNull other && Equals(other);
        }
        public override int GetHashCode() => base.GetHashCode();
        public static bool operator ==(NekoNull left, NekoNull right) => Equals(left, right);

        public static bool operator !=(NekoNull left, NekoNull right) => !Equals(left, right);

        #endregion
    }
}