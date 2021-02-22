using System.Collections.Generic;

namespace ProjectX.Core.Cache
{
    public interface IScopedCache<TKey, TValue> : IDictionary<TKey, TValue> {}

    public class ScopedCache<TKey, TValue> : Dictionary<TKey, TValue>, IScopedCache<TKey, TValue> {}
}
