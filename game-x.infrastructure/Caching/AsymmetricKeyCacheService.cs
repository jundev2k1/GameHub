using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Exceptions;
using game_x.domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace game_x.infrastructure.Caching;

public sealed class AsymmetricKeyCacheService(
    GameXContext context,
    IMemoryCache cache) : CacheService(cache), IAsymmetricKeyCacheService
{
    private const string CacheKey = "asymmetricKey:list";

    public void Refresh()
    {
        var keys = context.AsymmetricKey
            .AsNoTracking()
            .ToDictionary(
                x => BuildCacheKey(x.Name, x.KeyType, x.Algorithm),
                x => x.KeyValue);
        Set(CacheKey, keys);
    }

    public string GalaxyPrivateKey => GetKey(AsymmetricKeyNames.GalaxyPay, KeyType.Private, AsymmetricType.ECDSA);
    public string GalaxyPublicKey  => GetKey(AsymmetricKeyNames.GalaxyPay, KeyType.Public,  AsymmetricType.ECDSA);
    public string UxmPublicKey  => GetKey(AsymmetricKeyNames.Uxm, KeyType.Public,  AsymmetricType.ECDSA);

    private Dictionary<string, string>? AsymmetricKeys => Get<Dictionary<string, string>?>(CacheKey);

    private string GetKey(string name, KeyType keyType, string algorithm)
    {
        if (AsymmetricKeys is null)
            Refresh();

        var key = BuildCacheKey(name, keyType, algorithm);
        if (AsymmetricKeys?.TryGetValue(key, out var value) == true)
            return value;

        throw new NotFoundException($"Asymmetric key not found: {key}");
    }

    private static string BuildCacheKey(string name, KeyType keyType, string algorithm)
        => $"{name}_{keyType}_{algorithm}";
}
