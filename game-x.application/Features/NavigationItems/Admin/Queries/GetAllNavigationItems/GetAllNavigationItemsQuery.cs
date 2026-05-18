using game_x.application.Features.NavigationItems.Dtos;

namespace game_x.application.Features.NavigationItems.Admin.Queries.GetAllNavigationItems;

public record GetAllNavigationItemsQuery() : IQuery<NavigationItemDto[]>;
