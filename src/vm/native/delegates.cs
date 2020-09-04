namespace Neko.NativeRing
{
    using Base;

    internal unsafe delegate NekoObject oic(NekoValue* value);

    internal unsafe delegate void* nfd0();
    internal unsafe delegate void* nfd1(void* x1);
    internal unsafe delegate void* nfd2(void* x1, void* x2);
    internal unsafe delegate void* nfd3(void* x1, void* x2, void* x3);
    internal unsafe delegate void* nfd4(void* x1, void* x2, void* x3, void* x4);
    internal unsafe delegate void* nfd5(void* x1, void* x2, void* x3, void* x4, void* x5);

    internal unsafe delegate void nad0();
    internal unsafe delegate void nad1(void* x1);
    internal unsafe delegate void nad2(void* x1, void* x2);
    internal unsafe delegate void nad3(void* x1, void* x2, void* x3);
    internal unsafe delegate void nad4(void* x1, void* x2, void* x3, void* x4);
    internal unsafe delegate void nad5(void* x1, void* x2, void* x3, void* x4, void* x5);
}