namespace WeatherMonitor.Cache;
public class Memoizer<TKey, TResult> where TKey : notnull
{
    private readonly Func<TKey, TResult> _func;
    private readonly int _maxSize;
    private readonly Dictionary<TKey, TResult> _cache = new();
    private readonly Dictionary<TKey, DateTime> _access = new();
    private int _hits = 0;
    private int _misses = 0;
    public Memoizer(
        Func<TKey, TResult> func,
        int maxSize = int.MaxValue)
    {
        _func = func;
        _maxSize = maxSize;
    }
    public TResult Get(TKey key)
    {
        if (_cache.TryGetValue(key, out var cached))
        {
            _hits++;
            _access[key] = DateTime.UtcNow;
            return cached;
        }
        _misses++;
        if (_cache.Count >= _maxSize)
            EvictLeastRecentlyUsed();
        var result = _func(key);
        _cache[key] = result;
        _access[key] = DateTime.UtcNow;

        return result;
    }
    public int Count => _cache.Count;
    public double HitRate => (_hits + _misses) == 0
        ? 0
        : (double)_hits / (_hits + _misses);
    public void Clear()
    {
        _cache.Clear();
        _access.Clear();
    }
    private void EvictLeastRecentlyUsed()
    {
        if (_cache.Count == 0) return;
        TKey oldestKey = default!;
        DateTime oldestAccess = DateTime.MaxValue;
        foreach (var item in _access)
        {
            if (item.Value < oldestAccess)
            {
                oldestKey = item.Key;
                oldestAccess = item.Value;
            }
        }
        _cache.Remove(oldestKey);
        _access.Remove(oldestKey);
    }
}