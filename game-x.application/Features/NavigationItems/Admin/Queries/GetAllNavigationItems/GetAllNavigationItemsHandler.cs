using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.NavigationItems.Dtos;

namespace game_x.application.Features.NavigationItems.Admin.Queries.GetAllNavigationItems;

public sealed class GetAllNavigationItemsHandler(
    INavigationItemRepo navigationItemRepo,
    IFileManagerCacheService fileManagerCache) : IQueryHandler<GetAllNavigationItemsQuery, NavigationItemDto[]>
{
    public async Task<NavigationItemDto[]> Handle(GetAllNavigationItemsQuery request, CancellationToken ct = default)
    {
        var items = await navigationItemRepo.GetAllAsync(ct);
        var result = items.Adapt<NavigationItemDto[]>();
        foreach (var item in result)
        {
            if (item.Icon is null) continue;

            item.IconUrl = await fileManagerCache.GetFileUrl(item.Icon, ct);
        }

        return result;
    }
}
