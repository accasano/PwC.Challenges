using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace PwC.CarRental.UnitTests.Fakes;

public class FakeMemoryCache : IMemoryCache
{
    private readonly Dictionary<object, object> _store = [];

    public ICacheEntry CreateEntry(object key)
    {
        var entry = new FakeCacheEntry(key);
        _store[key] = entry.Value;
        return entry;
    }

    public void Remove(object key) => _store.Remove(key);

    public bool TryGetValue(object key, out object value) => _store.TryGetValue(key, out value);

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    private class FakeCacheEntry(object key) : ICacheEntry
    {
        public object Key { get; } = key;
        public object Value { get; set; }
        public DateTimeOffset? AbsoluteExpiration { get; set; }
        public TimeSpan? SlidingExpiration { get; set; }
        public IList<IChangeToken> ExpirationTokens => [];
        public IList<PostEvictionCallbackRegistration> PostEvictionCallbacks => [];
        public CacheItemPriority Priority { get; set; }
        public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }
        public long? Size { get; set; }
        public void Dispose() { }
    }
}
