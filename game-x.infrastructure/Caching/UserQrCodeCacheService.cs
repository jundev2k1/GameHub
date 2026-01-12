using game_x.application.Contract.Infrastructure.Caching;
using Microsoft.Extensions.Caching.Memory;

namespace game_x.infrastructure.Caching;

public class UserQrCodeCacheService(IMemoryCache cache) : CacheService(cache), IUserQrCodeCacheService
{
    private readonly string _prefix = "userQrCode:";
    private readonly int _timeSecondExpiration = 30;

    public string? GetToken(string userId)
    {
        var cacheKey = $"{_prefix}userId:${userId}";
        return Get<string>(cacheKey);
    }

    public string? GetUserId(string token)
    {
        var cacheKey = $"{_prefix}token:${token}";
        return Get<string>(cacheKey);
    }

    public void SetToken(string userId, string token, int? expiresIn = null)
    {
        // Store or update the token in memory cache with expiration
        var options = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromSeconds(expiresIn ?? _timeSecondExpiration));
        var cacheKey = $"{_prefix}userId:${userId}";
        Set(cacheKey, token, options);
    }

    public void SetUserId(string token, string userId, int? expiresIn = null)
    {
        // Store or update the token in memory cache with expiration
        var options = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromSeconds(expiresIn ?? _timeSecondExpiration));

        var cacheKey = $"{_prefix}token:${token}";
        Set(cacheKey, userId, options);
    }

    public void RemoveToken(string userId)
    {
        var cacheKey = $"{_prefix}userId:${userId}";
        Remove(cacheKey);
    }

    public void RemoveUserId(string token)
    {
        var cacheKey = $"{_prefix}token:${token}";
        Remove(cacheKey);
    }
}
