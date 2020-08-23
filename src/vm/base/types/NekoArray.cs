namespace Neko.Base
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using NativeRing;
    
    public unsafe class NekoArray : NekoObject, IEnumerable<NekoObject>, INativeCast<_neko_array>, ICollection
    {
        internal NekoArray(NekoValue* value) : base(value) => 
            NekoAssert.IsArray(value);
        public NekoObject this[int i]
        {
            get => GetByIndex(i);
            set => SetByIndex(i, value);
        }
        public void SetByIndex(int index, NekoObject obj) 
            => SetByIndexNative(index, obj);
        public NekoObject GetByIndex(int index) 
            => GetByIndexNative(index);
        public NekoValue* GetByIndexNative(int index) 
            => &((_neko_array*)@ref)->ptr[index];
        public void SetByIndexNative(int index, NekoValue* value)
        {
            var ptr = (void**) &((_neko_array*) @ref)->ptr;
            ptr[index] = value;
        }

        public _neko_array* AsInternal() 
            => (_neko_array*)@ref;

        #region Implementation of IEnumerable

        public IEnumerator<NekoObject> GetEnumerator()
        {
            for (var i = 0; i != Native.neko_val_array_size(this); i++)
                yield return GetByIndex(i);
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        #region Implementation of ICollection

        public void CopyTo(Array array, int index) 
            => throw new NotSupportedException();

        public int Count => Native.neko_val_array_size(this);
        public bool IsSynchronized => false;
        public object SyncRoot { get; } = new object();

        #endregion

        public static NekoArray Alloc(uint size)
        {
            var narr = Native.neko_alloc_array(size);
            var darr = new NekoArray(narr);
            for(var i=0;i<size;i++)
                darr[i] = Native.v_null();
            return darr;
        }
    }
}