using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.NavigationItems.Dtos;

namespace game_x.application.Features.NavigationItems.Admin.Queries.GetAllNavigationItems;

public sealed class GetAllNavigationItemsHandler(
    INavigationItemRepo navigationItemRepo) : IQueryHandler<GetAllNavigationItemsQuery, NavigationItemDto[]>
{
    public async Task<NavigationItemDto[]> Handle(GetAllNavigationItemsQuery request, CancellationToken ct = default)
    {
        var items = await navigationItemRepo.GetAllAsync(ct);
        return items.Adapt<NavigationItemDto[]>();
    }
}
