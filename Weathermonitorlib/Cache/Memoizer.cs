namespace WeatherMonitor.Cache;

public class Memoizer<TKey, TResult> where TKey : notnull
{
    private readonly Func<TKey, TResult> _func;
    private readonly Dictionary<TKey, TResult> _cache = new();
    private int _hits = 0;
    private int _misses = 0;
    public Memoizer(Func<TKey, TResult> func)
    {
        _func = func;
    }
    public TResult Get(TKey key)
    {
        if (_cache.TryGetValue(key, out var cached))
        {
            _hits++;
            return cached;
        }
        _misses++;
        var result = _func(key);
        _cache[key] = result;
        return result;
    }
    public int Count => _cache.Count;
    public double HitRate => (_hits + _misses) == 0
        ? 0
        : (double)_hits / (_hits + _misses);
    public void Clear()
    {
        _cache.Clear();
    }
}