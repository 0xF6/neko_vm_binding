namespace Neko.Base
{
    using NativeRing;

    public abstract unsafe class NekoBehaviour : INekoDisposable
    {
        protected internal NekoValue* @ref;
        protected internal NekoBehaviour(NekoValue* value) => @ref = value;

        ~NekoBehaviour() => (this as INekoDisposable)._release();
        void INekoDisposable._release() => @ref = null;

        public static implicit operator NekoValue*(NekoBehaviour behaviour) => behaviour.@ref;
    }
}