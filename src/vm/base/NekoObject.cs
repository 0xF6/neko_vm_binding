namespace Neko.Base
{
    using NativeRing;

    public unsafe class NekoObject : NekoBehaviour
    {
        protected internal NekoObject(NekoValue* value) : base(value) { }

        public override string ToString() => $"NekoObject {@ref->GetValueType()}";
        public override int GetHashCode() => Native.neko_val_hash(this.@ref);
    }
}