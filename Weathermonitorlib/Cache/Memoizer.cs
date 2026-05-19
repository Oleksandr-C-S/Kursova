namespace WeatherMonitor.Cache;
public enum EvictionPolicy
{
    LRU,      
    LFU,       
    TimeBased  
}
public class Memoizer<TKey, TResult> where TKey : notnull 
{
    private readonly Func<TKey, TResult> _func;
    private readonly int _maxSize;   
    private readonly EvictionPolicy _policy; 
    private readonly TimeSpan _ttl;  

    private readonly Dictionary<TKey, TResult>  _cache   = new(); 
    private readonly Dictionary<TKey, int>       _freq    = new(); 
    private readonly Dictionary<TKey, DateTime>  _access  = new(); 
    private readonly Dictionary<TKey, DateTime>  _created = new();
    private int _hits   = 0; 
    private int _misses = 0; 

    public Memoizer(
        Func<TKey, TResult> func,
        int maxSize = int.MaxValue,  
        EvictionPolicy policy = EvictionPolicy.LRU,
        TimeSpan ttl = default)     
    {
        _func    = func;
        _maxSize = maxSize;
        _policy  = policy;
        _ttl     = ttl == default ? TimeSpan.FromMinutes(5) : ttl;
    }

    public TResult Get(TKey key)
    {
        if (_policy == EvictionPolicy.TimeBased)
            PurgeExpired();

        if (_cache.TryGetValue(key, out var cached))
        {
            _hits++;
            _freq[key]   = _freq.GetValueOrDefault(key) + 1;
            _access[key] = DateTime.UtcNow; 
            return cached; 
        }

        _misses++;
        if (_cache.Count >= _maxSize)
            Evict();

        var result = _func(key);

        _cache[key]   = result;
        _freq[key]    = 1;
        _access[key]  = DateTime.UtcNow;
        _created[key] = DateTime.UtcNow;

        return result;
    }
    public int Count => _cache.Count; 
    public double HitRate => (_hits + _misses) == 0 ? 0 : (double)_hits / (_hits + _misses);
    public void Clear()
    {
        _cache.Clear();
        _freq.Clear();
        _access.Clear();
        _created.Clear();
    }

    private void Evict()
    {
        if (_cache.Count == 0) return;

        TKey? victim = _policy switch
        {
            EvictionPolicy.LFU => MinByValue(_freq), 
            _                  => MinByValue(_access),
        };

        if (victim is null) return;
        _cache.Remove(victim);
        _freq.Remove(victim);
        _access.Remove(victim);
        _created.Remove(victim);
    }
    private void PurgeExpired()
    {
        var now = DateTime.UtcNow;
        foreach (var key in _created.Keys.ToList())
        {
            if (now - _created[key] > _ttl)
            {
                _cache.Remove(key);
                _freq.Remove(key);
                _access.Remove(key);
                _created.Remove(key);
            }
        }
    }
    private static TKey? MinByValue<TV>(Dictionary<TKey, TV> dict) where TV : IComparable<TV>
    {
        if (dict.Count == 0) return default;

        TKey   bestKey = default!;
        TV?    bestVal = default;

        foreach (var kv in dict)
        {
            if (bestVal is null || kv.Value.CompareTo(bestVal) < 0)
            {
                bestKey = kv.Key;
                bestVal = kv.Value;
            }
        }
        return bestKey;
    }
}