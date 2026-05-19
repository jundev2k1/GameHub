using game_x.application.Features.NavigationItems.Dtos;

namespace game_x.application.Features.NavigationItems.Admin.Queries.GetNavigationItemDetail;

public record GetNavigationItemDetailQuery(Guid Id) : IQuery<NavigationItemDetailDto>;
