namespace Neko.NativeRing
{
    using Neko.Base;
    using System.Reflection;
    internal static unsafe class __unsafe_cast
    {
        public static void* _cdel(object o)
        {
            if (o is NekoBehaviour no)
                return no.@ref;
            return Pointer.Unbox(o);
        }
        public static object _cwel(void* o) => Pointer.Box(o, typeof(void*));

        public static void* _cmp(MethodInfo info, params object[] args) => _cdel(info.Invoke(null, args));
        public static void _cmv(MethodInfo info, params object[] args) => info.Invoke(null, args);
    }
}