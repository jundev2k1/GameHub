using Microsoft.Extensions.Caching.Memory;

namespace game_x.infrastructure.Security.HMac;

public interface IHmacNonceStore
{
    Task<bool> TryUseNonceAsync(string nonce, TimeSpan ttl);
}

public sealed class HmacNonceStore(IMemoryCache memoryCache) : IHmacNonceStore
{
    public Task<bool> TryUseNonceAsync(string nonce, TimeSpan ttl)
    {
        bool isNew = false;
        var value = memoryCache.GetOrCreate(nonce, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = ttl;
            isNew = true;
            return true;
        });
        return Task.FromResult(isNew);
    }
}
