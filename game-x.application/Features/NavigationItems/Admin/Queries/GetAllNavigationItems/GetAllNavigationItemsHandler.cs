using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.NavigationItems.Dtos;

namespace game_x.application.Features.NavigationItems.Admin.Queries.GetAllNavigationItems;

public sealed class GetAllNavigationItemsHandler(
    IUserAccessor userAccessor,
    INavigationItemRepo navigationItemRepo,
    IFileManagerCacheService fileManagerCache,
    IGameProviderCacheService gameProviderCache) : IQueryHandler<GetAllNavigationItemsQuery, NavigationItemDto[]>
{
    public async Task<NavigationItemDto[]> Handle(GetAllNavigationItemsQuery request, CancellationToken ct = default)
    {
        var lang = userAccessor.GetLanguage();
        var cateDic = gameProviderCache.CategoryList
            .ToDictionary(cate => cate.LocalId, cate => cate);

        var items = await navigationItemRepo.GetAllAsync(ct);
        var result = items.Adapt<NavigationItemDto[]>();
        foreach (var item in result)
        {
            if (item.TargetLocalId.HasValue && cateDic.TryGetValue(item.TargetLocalId.Value, out var cate))
                item.TargetId = cate.Id;

            if (item.Icon is not null)
                item.IconUrl = await fileManagerCache.GetFileUrl(item.Icon, ct);
        }

        return result;
    }
}
