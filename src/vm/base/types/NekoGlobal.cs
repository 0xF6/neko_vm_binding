namespace Neko.Base
{
    using System;
    using NativeRing;

    internal sealed unsafe class NekoGlobal : NekoRuntimeObject
    {
        internal NekoGlobal(NekoValue* value) : base(value) { }

        public NekoValueType @typeof(NekoValue* value) =>
            (NekoValueType)((NekoInt)(new NekoFunction("typeof", base["typeof"].@ref))
                .InvokeWithNative(value) /* wtf */ - 1);

        public NekoString smake(int size) =>
            (NekoString)(new NekoFunction(nameof(smake), base[nameof(smake)].@ref))
                .InvokeWithNative(Native.neko_alloc_int(size));
        public NekoInt ssize(NekoString str) =>
            (NekoInt)(new NekoFunction(nameof(ssize), base[nameof(ssize)].@ref))
            .InvokeWithNative(str.@ref);
        [Obsolete("Not working...")]
        public NekoRuntimeObject @new(NekoObject value) =>
            (NekoRuntimeObject)new NekoFunction("new", base["new"].@ref)
                .Invoke(value);
    }
}