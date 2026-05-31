using game_x.application.Contract.Infrastructure.Caching;
using Microsoft.Extensions.Caching.Memory;

namespace game_x.infrastructure.Caching;

public sealed class EmailCacheService(IMemoryCache cache) : CacheService(cache), IEmailCacheService
{
    private readonly string _prefix = "email:";

    public void SetCode(string email, string purpose, string code, TimeSpan? expiresIn = null)
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiresIn
        };
        var cacheKey = $"{_prefix}${email}:code:${purpose}";
        Set(cacheKey, code, options);
    }

    public void RemoveCode(string email, string purpose)
    {
        var cacheKey = $"{_prefix}${email}:code:${purpose}";
        Remove(cacheKey);
    }

    public string? GetCode(string email, string purpose)
    {
        var cacheKey = $"{_prefix}${email}:code:${purpose}";
        return Get<string>(cacheKey);
    }
}
