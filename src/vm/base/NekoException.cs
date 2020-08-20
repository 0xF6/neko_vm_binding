namespace Neko.Base
{
    using System;
    using System.IO;

    public abstract class NekoException : Exception
    {
        protected NekoException() { }
        protected NekoException(string message) : base(message) { }
    }

    public sealed class ModuleLoadNekoException : NekoException
    {
        public ModuleLoadNekoException(FileInfo file, string exception) 
            : base($"Failed load module '{file.Name}' due to '{exception}'.") { }
    }
    public sealed class IsNotAFunctionNekoException : NekoException
    {
        public IsNotAFunctionNekoException(string functionName) 
            : base($"'{functionName}' is not a function.") { }
    }
    public sealed class TypeIsNotSupportNekoException : NekoException
    {
        public TypeIsNotSupportNekoException(string ty) 
            : base($"'{ty}' is support type.") { }
    }
}