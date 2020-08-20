namespace Neko.Base
{
    using NativeRing;

    public sealed unsafe class NekoModule : NekoBehaviour
    {
        internal NekoModule(NekoValue* value) : base(value) { }

        public NekoFunction this[string name] => NekoFunction.Create(this, name);
    }
}