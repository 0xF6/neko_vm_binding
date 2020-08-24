namespace Neko.extensions
{
    using System;

    public static class TypeExtensions
    {
        // wtf
        public static bool IsDelegate(this Type t)
        {
            try
            {
                return t.GetMethod("Invoke") != null;
            }
            catch
            {
                return false;
            }
        }
    }
}