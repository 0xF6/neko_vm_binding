namespace Neko.Base
{
    using System;
    using System.IO;
    using NativeRing;

    public abstract class NekoException : Exception
    {
        protected NekoException() { }
        protected NekoException(string message) : base(message) { }
    }

    public sealed class NekoVMException : NekoException
    {
        private readonly NekoRuntimeException _exception;

        internal NekoVMException(NekoRuntimeException exception) : base(exception.Message)
            => _exception = exception;

        public override string Source
            => $"{_exception.Function} in {_exception.File}:{_exception.Line}";
        public override string StackTrace
            => $"   at {Source}{Environment.NewLine}{base.StackTrace}";

        public override string ToString() => $"\nAssert Neko Exception [{_exception.Message}] \n{StackTrace}";
    }

    public sealed class ModuleLoadNekoException : NekoException
    {
        public ModuleLoadNekoException(FileInfo file, string exception)
            : base($"Failed load module '{file.Name}' due to '{exception}'.") { }
    }

    public sealed unsafe class InvalidTypeNekoException : NekoException
    {
        public InvalidTypeNekoException(NekoValueType target, NekoValue* value)
            : base($"Type '{NekoType.get_valtype(value)}' is not '{target}'") { }
    }
    public sealed class TypeIsNotSupportNekoException : NekoException
    {
        public TypeIsNotSupportNekoException(string ty)
            : base($"'{ty}' is support type.") { }
    }

    public sealed unsafe class NullReferenceNekoException<T> : NekoException
    {
        public NullReferenceNekoException(NekoValue* value)
            : base($"Address 0x{(IntPtr)value:X} is invalid and cannot convert to {typeof(T).Name} type.") { }
    }
    public sealed class InvalidArgumentNekoException : NekoException { }
}