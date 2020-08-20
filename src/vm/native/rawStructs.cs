namespace Neko.NativeRing
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public unsafe struct hcell
    {
        public int hkey;
        public NekoValue key;
        public NekoValue val;
        public hcell* next;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public unsafe struct vhash
    {
        public hcell** cells;
        public int ncells;
        public int nitems;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public ref struct vint32
    {
        public uint t;
        public int i;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public ref struct vfloat
    {
        public uint t;
        public double f;
    }
}