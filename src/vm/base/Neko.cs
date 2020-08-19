namespace Neko.Base
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using NativeRing;

    public unsafe class Neko : INekoDisposable
    {
        internal NekoVM* _vm;
        internal readonly IDictionary<string, NekoModule> modules = new Dictionary<string, NekoModule>();
        internal NekoLoader _loader { get; private set; }
        public Neko()
        {
            Native.neko_global_init();
            _vm = Native.neko_vm_alloc(IntPtr.Zero.ToPointer());
            Native.neko_vm_select(_vm);
            _loader = NekoLoader.CreateDefault();
        }



        public NekoModule LoadModule(FileInfo file) => _loader.Load(file);

        void INekoDisposable._release()
        {
            _vm = null;
            _loader = null;
            foreach (var (name, module) in modules) 
                (module as IDisposable).Dispose();
            Native.neko_global_free();
            NativeLibrary.Free(Native._ref);
        }
        ~Neko() => (this as INekoDisposable)._release();
    }
}