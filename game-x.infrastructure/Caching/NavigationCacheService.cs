using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.NavigationItems.Dtos;
using Microsoft.Extensions.Caching.Memory;

namespace game_x.infrastructure.Caching;

public sealed class NavigationCacheService(
    IMemoryCache cache,
    INavigationItemRepo navigationItemRepo) : CacheService(cache), INavigationCacheService
{
    private readonly string _prefixCache = "navigation:";

    public async Task RefreshNavigationItemsAsync(CancellationToken ct = default)
    {
        var cacheKey = $"{_prefixCache}:items";
        var items = await navigationItemRepo.GetAllAsync(ct);
        Set(cacheKey, items.Adapt<NavigationItemDto[]>());
    }

    public NavigationItemDto[] NavigationItems
        => Get<NavigationItemDto[]>($"{_prefixCache}:items") ?? [];
}
