namespace Neko.NativeRing
{
    public unsafe interface INativeCast<T> where T : unmanaged
    {
        T* AsInternal();
    }
}