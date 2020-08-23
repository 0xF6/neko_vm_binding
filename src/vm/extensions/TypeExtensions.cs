namespace Neko.extensions
{
    using System;

    public static class TypeExtensions
    {
        // wtf
        public static bool IsDelegate(this Type t) 
            => t.GetMethod("Invoke") != null;
    }
}