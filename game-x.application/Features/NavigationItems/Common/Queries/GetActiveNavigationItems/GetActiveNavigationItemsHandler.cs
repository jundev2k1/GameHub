using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Features.NavigationItems.Dtos;

namespace game_x.application.Features.NavigationItems.Common.Queries.GetActiveNavigationItems;

public sealed class GetActiveNavigationItemsHandler(
    IUserAccessor userAccessor,
    INavigationCacheService navigationCache,
    IFileManagerCacheService fileManagerCache) : IQueryHandler<GetActiveNavigationItemsQuery, NavigationItemDto[]>
{
    public async Task<NavigationItemDto[]> Handle(GetActiveNavigationItemsQuery request, CancellationToken ct = default)
    {
        var lang = userAccessor.GetLanguage();
        var items = await MapToResultAsync(navigationCache.NavigationItems, lang);
        return items;
    }

    private async Task<NavigationItemDto[]> MapToResultAsync(NavigationItemDto[] items, string language)
    {
        foreach (var item in items)
        {
            // Map icon URL
            item.IconUrl = await fileManagerCache.GetFileUrl(item.Icon);

            // Map translation
            if (item.NavigationTranslations.TryGetValue(language, out NavigationItemTranslationInfo? translation))
            {
                item.Title = translation.Title;
            }
        }

        return items;
    }
}
