using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace game_x.infrastructure.Caching;

public sealed class AsymmetricKeyCacheService(
    GameXContext context,
    IMemoryCache cache) : CacheService(cache), IAsymmetricKeyCacheService
{
    private const string CacheKey = "asymmetricKey:list";
    private Dictionary<string, string>? DataSource => Get<Dictionary<string, string>?>(CacheKey);

    public async Task RefreshAsync()
    {
        var keys = await context.AsymmetricKeys
            .AsNoTracking()
            .ToDictionaryAsync(
                ak => BuildCacheKey(ak.Name, ak.KeyType, ak.Algorithm),
                ak => ak.KeyValue);
        Set(CacheKey, keys);
    }

    public string GameXPrivateKey => GetKey(AsymmetricKeyNames.GameX, AsymmetricKeyType.Private, AsymmetricType.ECDSA);
    public string GameXPublicKey  => GetKey(AsymmetricKeyNames.GameX, AsymmetricKeyType.Public,  AsymmetricType.ECDSA);
    public string UxmPublicKey  => GetKey(AsymmetricKeyNames.Uxm, AsymmetricKeyType.Public,  AsymmetricType.ECDSA);
    public string FastPayPublicKey => GetKey(AsymmetricKeyNames.FastPay, AsymmetricKeyType.Public,  AsymmetricType.ECDSA);
    public string SlotPublicKey => GetKey(AsymmetricKeyNames.Slot, AsymmetricKeyType.Public,  AsymmetricType.ECDSA);

    private string GetKey(string name, AsymmetricKeyType keyType, string algorithm)
    {
        if (DataSource is null)
            return string.Empty;

        var key = BuildCacheKey(name, keyType, algorithm);
        if (DataSource?.TryGetValue(key, out var value) == true)
            return value;

        throw new NotFoundException($"Asymmetric key not found: {key}");
    }

    private static string BuildCacheKey(string name, AsymmetricKeyType keyType, string algorithm)
        => $"{name}_{keyType}_{algorithm}";
}
