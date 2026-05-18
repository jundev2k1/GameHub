using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Features.NavigationItems.Dtos;

namespace game_x.application.Features.NavigationItems.Common.Queries.GetActiveNavigationItems;

public sealed class GetActiveNavigationItemsHandler(
    IUserAccessor userAccessor,
    INavigationCacheService navigationCache) : IQueryHandler<GetActiveNavigationItemsQuery, NavigationItemDto[]>
{
    public async Task<NavigationItemDto[]> Handle(GetActiveNavigationItemsQuery request, CancellationToken ct = default)
    {
        var lang = userAccessor.GetLanguage();
        var items = MapToResult(navigationCache.NavigationItems, lang).ToArray();
        return await Task.FromResult(items);
    }

    private static IEnumerable<NavigationItemDto> MapToResult(NavigationItemDto[] items, string language)
    {
        foreach (var item in items)
        {
            if (item.NavigationTranslations.TryGetValue(language, out NavigationItemTranslationInfo? translation))
            {
                item.Title = translation.Title;
                yield return item;
            }
        }
    }
}
