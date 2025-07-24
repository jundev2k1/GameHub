using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace game_x.infrastructure.Caching;

public abstract class CacheService
{
    private readonly IMemoryCache _cache;

    public CacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public bool TryGetValue<T>(string key, out T? value)
    {
        value = default;

        if (!_cache.TryGetValue(key, out var cached))
            return false;

        if (cached is T tValue)
        {
            value = tValue;
            return true;
        }

        if (cached is string json)
        {
            try
            {
                value = JsonSerializer.Deserialize<T>(json);
                return value is not null;
            }
            catch (JsonException) { }
        }

        return false;
    }

    public T? Get<T>(string key)
    {
        return TryGetValue<T>(key, out var value) ? value : default;
    }

    protected void Set(string cacheKey, object value, MemoryCacheEntryOptions? options = null)
    {
        _cache.Set(cacheKey, value, options);
    }

    protected void Remove(string cacheKey)
    {
        _cache.Remove(cacheKey);
    }
}
