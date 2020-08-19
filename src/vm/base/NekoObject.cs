namespace Neko.Base
{
    public unsafe class NekoObject : NekoBehaviour
    {
        protected internal NekoObject(NekoValue* value) : base(value) { }

        public override string ToString() => $"NekoObject {@ref->GetValueType()}";
    }
}