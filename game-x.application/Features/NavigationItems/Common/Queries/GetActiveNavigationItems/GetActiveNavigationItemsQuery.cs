using game_x.application.Features.NavigationItems.Dtos;

namespace game_x.application.Features.NavigationItems.Common.Queries.GetActiveNavigationItems;

public record GetActiveNavigationItemsQuery() : IQuery<NavigationItemDto[]>;
