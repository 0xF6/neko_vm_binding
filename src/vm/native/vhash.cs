namespace Neko
{
    public unsafe struct vhash
    {
        public hcell** cells;
        public int ncells;
        public int nitems;
    }
}