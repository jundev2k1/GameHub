using game_x.application.Features.Rewards.Dtos;

namespace game_x.application.Contract.Infrastructure.Caching.Rewards;

public interface ICatalogItemCacheService
{
    Task RefreshCache(CancellationToken ct = default);

    Task<CatalogItemDto[]?> GetAll(CancellationToken ct = default);
}