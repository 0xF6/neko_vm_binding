namespace Neko.Base
{
    using System.IO;
    using NativeRing;

    public sealed unsafe class NekoLoader : NekoObject
    {
        internal NekoLoader(NekoValue* value) : base(value) { }

        public static NekoLoader CreateDefault()
            => new NekoLoader(Native.neko_default_loader(null, 0));

        internal NekoModule Load(FileInfo file)
        {
            var args = stackalloc NekoValue*[2];
            var exception = (NekoValue*)null;
            args[0] = Native.neko_alloc_string(file.ToString());
            args[1] = this.@ref;
                
            var a1 = Native.neko_val_id("loadmodule");
            var a2 = Native.neko_val_field(this.@ref, a1);
            var result = Native.neko_val_callEx(this.@ref, a2, args,2, ref exception);
            if (exception == null || exception->IsNull()) 
                return new NekoModule(result);
            var b = Native.neko_alloc_buffer(null);
            Native.neko_val_buffer(b, exception);
            var raw = Native.neko_buffer_to_string(b);
            throw new ModuleLoadNekoException(file, NekoString.GetString(raw));
        }
    }
}