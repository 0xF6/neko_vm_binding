namespace Neko.NativeRing
{
    public unsafe struct hcell
    {
        public int hkey;
        public NekoValue key;
        public NekoValue val;
        public hcell* next;
    }
}