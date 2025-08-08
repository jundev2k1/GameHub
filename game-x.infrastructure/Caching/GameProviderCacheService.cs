using game_x.application.Contract.Infrastructure.Caching;
using Microsoft.Extensions.Caching.Memory;

namespace game_x.infrastructure.Caching;

public sealed class GameProviderCacheService(IMemoryCache cache) : CacheService(cache), IGameProviderCacheService
{
    private readonly string _prefixCache = "external:game-provider:";

    public string Language
    {
        get { return Get<string>($"{_prefixCache}:language") ?? "zh-Hant"; }
        set
        {
            var lang = (value != "zh-Hant" || value != "zh-Hants") ? value : "zh-Hant";
            Set($"{_prefixCache}:language", lang);
        }
    }
}
