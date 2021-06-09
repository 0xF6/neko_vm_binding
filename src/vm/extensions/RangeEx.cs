namespace Neko
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class RangeEx
    {
        public static IEnumerable<T> ForEach<T>(this Range range, Func<int, T> functor)
            => Enumerable.Range(range.Start.Value, range.End.Value - range.Start.Value).Select(functor);
        public static IEnumerator<int> GetEnumerator(this Range range)
            => Enumerable.Range(range.Start.Value, range.End.Value - range.Start.Value).GetEnumerator();
    }
}