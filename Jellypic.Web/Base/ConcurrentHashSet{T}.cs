using System.Collections.Generic;
using Jellypic.Web.Base;

namespace System.Collections.Concurrent
{
    public class ConcurrentHashSet<T> : Enumerable<T>
    {
        public void Add(T value) =>
            Values.AddOrUpdate(value, 0, (x, y) => 0);

        public override IEnumerator<T> GetEnumerator() =>
            Values.Keys.GetEnumerator();

        ConcurrentDictionary<T, byte> Values { get; } =
            new ConcurrentDictionary<T, byte>();
    }
}
