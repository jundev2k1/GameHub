using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.NavigationItems.Dtos;

namespace game_x.application.Features.NavigationItems.Admin.Queries.GetNavigationItemDetail;

public sealed class GetNavigationItemDetailHandler(
    INavigationItemRepo navigationItemRepo,
    IFileManagerCacheService fileManagerCache) : IQueryHandler<GetNavigationItemDetailQuery, NavigationItemDetailDto>
{
    public async Task<NavigationItemDetailDto> Handle(GetNavigationItemDetailQuery request, CancellationToken ct = default)
    {
        var targetNavigationItem = await navigationItemRepo.GetAsync(request.Id, ct);
        var result = targetNavigationItem.Adapt<NavigationItemDetailDto>();

        if (result.Icon is not null)
            result.IconUrl = await fileManagerCache.GetFileUrl(result.Icon, ct);

        return result;
    }
}
