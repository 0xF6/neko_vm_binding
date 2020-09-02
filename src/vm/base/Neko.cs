namespace Neko.Base
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using NativeRing;

    public unsafe class Neko : INekoDisposable, INativeCast<_neko_vm>
    {
        internal NekoVM* _vm;
        internal readonly IDictionary<string, NekoModule> modules = new Dictionary<string, NekoModule>();
        internal NekoLoader _loader { get; private set; }
        public int ThreadID { get; internal set; }
        public Neko()
        {
            ThreadID = Thread.CurrentThread.ManagedThreadId;
            Native.neko_global_init();
            _vm = Native.neko_vm_alloc(IntPtr.Zero.ToPointer());
            Native.neko_vm_select(_vm);
            _loader = NekoLoader.CreateDefault();
        }

        public NekoModule LoadModule(string path)
        {
            var file = new FileInfo(path);
            if (!file.Exists)
                throw new Exception($"File '{path}' not exists.");
            return LoadModule(file);
        }
        public NekoModule LoadModule(FileInfo file)
        {
            GuardBarrier();
            return _loader.Load(file);
        }
        public void GuardBarrier()
        {
            ThreadID = Thread.CurrentThread.ManagedThreadId;
            Native.neko_vm_select(_vm);
        }

        void INekoDisposable._release()
        {
            GuardBarrier();
            _vm = null;
            _loader = null;
            foreach (var (name, module) in modules) 
                (module as IDisposable).Dispose();
            Native.neko_global_free();
        }

        public _neko_vm* AsInternal() => (_neko_vm*) _vm;
        ~Neko() => (this as INekoDisposable)._release();
    }
}