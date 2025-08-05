using game_x.application.Contract.Infrastructure.Caching;
using Microsoft.Extensions.Caching.Memory;

namespace game_x.infrastructure.Caching;

public sealed class ResetTokenCacheService(IMemoryCache cache) : CacheService(cache), IResetTokenCacheService
{
    private const string KeyPrefix = "reset-token:";

    public void StoreToken(string token, string email, TimeSpan? expiresIn = null)
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiresIn ?? TimeSpan.FromMinutes(5),
        };
        Set(GetKey(token), email, options);
    }

    public string? GetEmailByToken(string token)
    {
        return Get<string>(GetKey(token));
    }

    public void InvalidateToken(string token)
    {
        Remove(GetKey(token));
    }

    private static string GetKey(string token) => $"{KeyPrefix}{token}";
}
