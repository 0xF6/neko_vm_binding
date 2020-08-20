namespace Neko.NativeRing
{
    using System;

    public interface INekoDisposable : IDisposable
    {
        void _release();
        void IDisposable.Dispose()
        {
            _release();
            // ReSharper disable once GCSuppressFinalizeForTypeWithoutDestructor
            GC.SuppressFinalize(this);
        }
    }
}