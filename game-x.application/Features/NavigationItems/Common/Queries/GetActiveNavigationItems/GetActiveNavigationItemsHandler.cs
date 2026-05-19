using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Features.NavigationItems.Dtos;

namespace game_x.application.Features.NavigationItems.Common.Queries.GetActiveNavigationItems;

public sealed class GetActiveNavigationItemsHandler(
    IUserAccessor userAccessor,
    INavigationCacheService navigationCache,
    IFileManagerCacheService fileManagerCache,
    IGameProviderCacheService gameProviderCache) : IQueryHandler<GetActiveNavigationItemsQuery, NavigationItemDto[]>
{
    public async Task<NavigationItemDto[]> Handle(GetActiveNavigationItemsQuery request, CancellationToken ct = default)
    {
        var lang = userAccessor.GetLanguage();
        var cateDic = gameProviderCache.CategoryList
            .ToDictionary(cate => cate.LocalId, cate => cate);

        var items = navigationCache.NavigationItems
            .Where(i => i.IsActive)
            .OrderByDescending(i => i.Priority)
            .ToArray();
        foreach (var item in items)
        {
            if (item.NavigationTranslations.TryGetValue(lang, out var translation))
                item.Title = translation.Title;

            if (item.TargetLocalId.HasValue && cateDic.TryGetValue(item.TargetLocalId.Value, out var cate))
                item.TargetId = cate.Id;

            if (item.Icon is not null)
                item.IconUrl = await fileManagerCache.GetFileUrl(item.Icon, ct);
        }

        return items;
    }
}
