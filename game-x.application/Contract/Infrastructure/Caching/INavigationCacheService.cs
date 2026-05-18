using game_x.application.Features.Games.Dtos;
using game_x.application.Features.NavigationItems.Dtos;

namespace game_x.application.Contract.Infrastructure.Caching;

public interface INavigationCacheService
{
    Task RefreshNavigationItemsAsync(CancellationToken ct = default);
  
    NavigationItemDto[] NavigationItems { get; }
}
