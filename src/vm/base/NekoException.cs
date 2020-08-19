namespace Neko.Base
{
    using System;
    using System.IO;

    public abstract class NekoException : Exception
    {
        protected NekoException() { }
        protected NekoException(string message) : base(message) { }
    }

    public sealed class NekoModuleLoadException : NekoException
    {
        public NekoModuleLoadException(FileInfo file, string exception) 
            : base($"Failed load module '{file.Name}' due to '{exception}'.") { }
    }
}