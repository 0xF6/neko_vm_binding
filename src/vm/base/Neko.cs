namespace Neko.Base
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Threading;
    using NativeRing;
    using static NativeRing.Native;

    public unsafe class Neko : INekoDisposable, INativeCast<_neko_vm>
    {
        internal NekoVM* _vm;
        internal readonly IDictionary<string, NekoModule> modules = new Dictionary<string, NekoModule>();
        internal NekoLoader _loader { get; private set; }
        public int ThreadID { get; internal set; }
        internal NekoGlobal Global { get; }
        public Neko()
        {
            ThreadID = Thread.CurrentThread.ManagedThreadId;
            neko_global_init();
            _vm = neko_vm_alloc(IntPtr.Zero.ToPointer());
            neko_vm_select(_vm);
            _loader = NekoLoader.CreateDefault();
            Global = new NekoGlobal(get_neko_builtins()[0]);
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
            var result = _loader.Load(file);
            modules.Add(file.Name, result);
            return result;
        }
        public void GuardBarrier()
        {
            ThreadID = Thread.CurrentThread.ManagedThreadId;
            neko_vm_select(_vm);
        }


        

        public void MarshalGlobal(Type t)
        {
            if (!t.IsClass)
                throw new ArgumentException($"'{t.Name}' is not class.");
            if (!(t.IsAbstract && t.IsSealed))
                throw new ArgumentException($"'{t.Name}' is not static class.");

            GuardBarrier();

            var methods = t.GetMethods(BindingFlags.Public | BindingFlags.Static);
            var gen = get_neko_builtins();
            foreach (var info in methods)
            {
                var @params = info.GetParameters();
                if (@params.Length > 5)
                    continue;
                if (!@params.All(x => NekoType.IsCompatible(x)))
                    continue;
                if (!(info.ReturnType == typeof(void) || NekoType.IsCompatible(info.ReturnType)))
                    continue;
                var m = MakeNekoProxy(info);
                var f = neko_alloc_function((void*) MakeNekoProxy(info), (uint) @params.Length, $"${info.Name}");
                neko_alloc_field(gen[0], neko_val_id(info.Name), f);
            }
        }

        internal unsafe static nint MakeNekoProxy(MethodInfo method)
        {
            var @p = method.GetParameters();
            var f = method.ReturnType == typeof(void);

            static nint Link<D>(Delegate d) where D : Delegate 
                => Marshal.GetFunctionPointerForDelegate((D) d);

            unsafe static oic GetOpImplicit()
                => typeof(NekoObject).GetMethod("op_Implicit", new[] {typeof(NekoValue*)}).CreateDelegate<oic>();

            unsafe static object[] Populate(ParameterInfo[] args, params void*[] innerArgs) 
                => args.Select((x, i) => GetOpImplicit()((NekoValue*) innerArgs[i])).ToArray();

            // maybe this need refactoring(((9(9
            return @p.Length switch
            {
                0 when f => Link<nad0>((nad0)(() => __unsafe_cast._cmv(method))),
                1 when f => Link<nad1>((nad1)((v1) => __unsafe_cast._cmv(method, Populate(@p, v1)))),
                2 when f => Link<nad2>((nad2)((v1, v2) => __unsafe_cast._cmv(method, Populate(@p, v1, v2)))),
                3 when f => Link<nad2>((nad3)((v1, v2, v3) => __unsafe_cast._cmv(method, Populate(@p, v1, v2, v3)))),
                4 when f => Link<nad2>((nad4)((v1, v2, v3, v4) => __unsafe_cast._cmv(method, Populate(@p, v1, v2, v3, v4)))),
                5 when f => Link<nad2>((nad5)((v1, v2, v3, v4, v5) =>__unsafe_cast._cmv(method, Populate(@p, v1, v2, v3, v4, v5)))),

                0 when !f => Link<nfd0>((nfd0)(() => __unsafe_cast._cmp(method))),
                1 when !f => Link<nfd1>((nfd1)((v1) => __unsafe_cast._cmp(method, Populate(@p, v1)))),
                2 when !f => Link<nfd2>((nfd2)((v1, v2) => __unsafe_cast._cmp(method, Populate(@p, v1, v2)))),
                3 when !f => Link<nfd3>((nfd3)((v1, v2, v3) => __unsafe_cast._cmp(method, Populate(@p, v1, v2, v3)))),
                4 when !f => Link<nfd4>((nfd4)((v1, v2, v3, v4) => __unsafe_cast._cmp(method, Populate(@p, v1, v2, v3, v4)))),
                5 when !f => Link<nfd5>((nfd5)((v1, v2, v3, v4, v5) => __unsafe_cast._cmp(method, Populate(@p, v1, v2, v3, v4, v5)))),
                _ => throw new NotSupportedException("Params count too many. [p > 5]")
            };
        }

        void INekoDisposable._release()
        {
            GuardBarrier();
            neko_vm_select(null);
            _vm = null;
            _loader = null;
            foreach (var (_, module) in modules) 
                (module as IDisposable).Dispose();
            neko_global_free();
        }

        public _neko_vm* AsInternal() => (_neko_vm*) _vm;
        ~Neko() => (this as INekoDisposable)._release();
    }
}