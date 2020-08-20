namespace Neko.Base
{
    using NativeRing;

    public sealed unsafe class NekoModule : NekoBehaviour
    {
        internal NekoModule(NekoValue* value) : base(value) { }
    }
}