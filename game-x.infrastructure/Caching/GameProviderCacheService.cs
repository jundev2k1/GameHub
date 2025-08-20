using game_x.application.Contract.Infrastructure.Caching;
using Microsoft.Extensions.Caching.Memory;

namespace game_x.infrastructure.Caching;

public sealed class GameProviderCacheService(IMemoryCache cache) : CacheService(cache), IGameProviderCacheService
{
    private readonly string _prefixCache = "external:game-provider:";

    public string GetLanguage(string account)
    {
        var language = Get<string>($"{_prefixCache}:{account}:language") ?? "zh-Hant";
        return language;
    }

    public void SetLanguage(string account, string language)
    {
        Set($"{_prefixCache}:{account}:language", language);
    }

    public bool GetIsLoggedIn(string account)
    {
        var isLoggedIn = Get<bool>($"{_prefixCache}:{account}:is-logged-in");
        return isLoggedIn;
    }

    public void SetIsLoggedIn(string account, bool isLoggedIn)
    {
        Set($"{_prefixCache}:{account}:is-logged-in", isLoggedIn);
    }
}
